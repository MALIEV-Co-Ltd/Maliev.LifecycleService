namespace Maliev.LifecycleService.Tests.Unit;

/// <summary>
/// Protects the LifecycleService container and immutable-release workflow contracts.
/// </summary>
public sealed class ContainerWorkflowContractTests
{
    private static readonly string[] ImageWorkflows =
    [
        "ci-develop.yml",
        "ci-staging.yml",
        "ci-main.yml"
    ];

    /// <summary>
    /// Docker restore must select published shared packages without mutating source files in CI.
    /// </summary>
    [Fact]
    public void DockerAndDevelopWorkflow_UseExplicitPackageModeWithoutSourceMutation()
    {
        var dockerfile = ReadRepositoryFile("Maliev.LifecycleService.Api", "Dockerfile");
        var directoryProps = ReadRepositoryFile("Directory.Build.props");
        var workflow = ReadRepositoryFile(".github", "workflows", "ci-develop.yml");

        Assert.Contains("GITHUB_ACTIONS=true", dockerfile, StringComparison.Ordinal);
        Assert.Contains("COPY [\"Directory.Build.props\", \".\"]", dockerfile, StringComparison.Ordinal);
        Assert.Contains(
            "dotnet restore \"./Maliev.LifecycleService.Api/Maliev.LifecycleService.Api.csproj\"",
            dockerfile,
            StringComparison.Ordinal);
        Assert.Contains("ARG MESSAGING_CONTRACTS_VERSION=1.0.96-alpha", dockerfile, StringComparison.Ordinal);
        Assert.Contains("ARG SERVICE_DEFAULTS_VERSION=1.0.89-alpha", dockerfile, StringComparison.Ordinal);
        Assert.Contains("-p:MessagingContractsVersion=$MESSAGING_CONTRACTS_VERSION", dockerfile, StringComparison.Ordinal);
        Assert.Contains("-p:ServiceDefaultsVersion=$SERVICE_DEFAULTS_VERSION", dockerfile, StringComparison.Ordinal);
        Assert.Contains(
            "<MessagingContractsVersion Condition=\"'$(MessagingContractsVersion)' == ''\">1.0.96-alpha</MessagingContractsVersion>",
            directoryProps,
            StringComparison.Ordinal);
        Assert.Contains(
            "<ServiceDefaultsVersion Condition=\"'$(ServiceDefaultsVersion)' == ''\">1.0.89-alpha</ServiceDefaultsVersion>",
            directoryProps,
            StringComparison.Ordinal);

        foreach (var project in new[]
                 {
                     "Maliev.LifecycleService.Api",
                     "Maliev.LifecycleService.Application",
                     "Maliev.LifecycleService.Infrastructure"
                 })
        {
            var projectFile = ReadRepositoryFile(project, $"{project}.csproj");
            Assert.Contains(
                "<PackageReference Include=\"Maliev.Aspire.ServiceDefaults\" Version=\"$(ServiceDefaultsVersion)\" />",
                projectFile,
                StringComparison.Ordinal);
            Assert.DoesNotContain(
                "<PackageReference Include=\"Maliev.Aspire.ServiceDefaults\" Version=\"$(SharedLibraryVersion)\" />",
                projectFile,
                StringComparison.Ordinal);
        }

        foreach (var project in new[]
                 {
                     "Maliev.LifecycleService.Application",
                     "Maliev.LifecycleService.Infrastructure"
                 })
        {
            var projectFile = ReadRepositoryFile(project, $"{project}.csproj");
            Assert.Contains(
                "<PackageReference Include=\"Maliev.MessagingContracts\" Version=\"$(MessagingContractsVersion)\" />",
                projectFile,
                StringComparison.Ordinal);
            Assert.DoesNotContain(
                "<PackageReference Include=\"Maliev.MessagingContracts\" Version=\"$(SharedLibraryVersion)\" />",
                projectFile,
                StringComparison.Ordinal);
        }

        Assert.DoesNotContain("sed -i", workflow, StringComparison.Ordinal);
        Assert.DoesNotContain("Switch to PackageReference", workflow, StringComparison.Ordinal);
    }

    /// <summary>
    /// Kubernetes probes, not a missing runtime executable, own container health evaluation.
    /// </summary>
    [Fact]
    public void FinalImage_ReliesOnKubernetesProbesInsteadOfUnavailableCurl()
    {
        var dockerfile = ReadRepositoryFile("Maliev.LifecycleService.Api", "Dockerfile");

        Assert.DoesNotContain("HEALTHCHECK", dockerfile, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("curl", dockerfile, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("USER $APP_UID", dockerfile, StringComparison.Ordinal);
        Assert.DoesNotContain("USER app", dockerfile, StringComparison.Ordinal);
    }

    /// <summary>
    /// Every image workflow must use short-lived WIF credentials with least privilege.
    /// </summary>
    [Theory]
    [MemberData(nameof(GetImageWorkflows))]
    public void ImageWorkflow_UsesFailClosedWifWithoutJsonCredentials(string workflowName)
    {
        var workflow = ReadRepositoryFile(".github", "workflows", workflowName);

        Assert.Contains("permissions:\n  contents: read", workflow, StringComparison.Ordinal);
        Assert.Contains("id-token: write", workflow, StringComparison.Ordinal);
        Assert.Contains("GCP_WORKLOAD_IDENTITY_PROVIDER", workflow, StringComparison.Ordinal);
        Assert.Contains("GCP_SERVICE_ACCOUNT", workflow, StringComparison.Ordinal);
        Assert.Contains("workload_identity_provider:", workflow, StringComparison.Ordinal);
        Assert.Contains("service_account:", workflow, StringComparison.Ordinal);
        Assert.DoesNotContain("credentials_json", workflow, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("GCP_SA_KEY", workflow, StringComparison.Ordinal);
    }

    /// <summary>
    /// Every generated GitOps change is evidence-only while LifecycleService apps remain disabled.
    /// </summary>
    [Theory]
    [MemberData(nameof(GetImageWorkflows))]
    public void ImageWorkflow_CreatesDraftDoNotMergeEvidenceWithoutAutoSyncClaims(string workflowName)
    {
        var workflow = ReadRepositoryFile(".github", "workflows", workflowName);

        Assert.Contains("--draft", workflow, StringComparison.Ordinal);
        Assert.Contains("--title \"[DO NOT MERGE]", workflow, StringComparison.Ordinal);
        Assert.Contains("_disabled_apps", workflow, StringComparison.Ordinal);
        Assert.Contains("changed=\"$(git diff --name-only)\"", workflow, StringComparison.Ordinal);
        Assert.Contains("test \"$changed\" = \"$expected\"", workflow, StringComparison.Ordinal);
        Assert.DoesNotContain("automatically sync", workflow, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Once merged", workflow, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Kustomize installation must be checksum verified and independent of deprecated Node actions.
    /// </summary>
    [Theory]
    [MemberData(nameof(GetImageWorkflows))]
    public void ImageWorkflow_UsesChecksumVerifiedKustomizeBinary(string workflowName)
    {
        var workflow = ReadRepositoryFile(".github", "workflows", workflowName);

        Assert.DoesNotContain("imranismail/setup-kustomize", workflow, StringComparison.Ordinal);
        Assert.Contains("KUSTOMIZE_VERSION: v5.8.1", workflow, StringComparison.Ordinal);
        Assert.Contains(
            "KUSTOMIZE_SHA256: 029a7f0f4e1932c52a0476cf02a0fd855c0bb85694b82c338fc648dcb53a819d",
            workflow,
            StringComparison.Ordinal);
        Assert.Contains("sha256sum --check", workflow, StringComparison.Ordinal);
    }

    /// <summary>
    /// Develop builds once with provenance; release workflows promote that verified digest.
    /// </summary>
    [Fact]
    public void ReleaseWorkflows_PreserveImmutableBuildAndPromotionBoundary()
    {
        var develop = ReadRepositoryFile(".github", "workflows", "ci-develop.yml");

        Assert.Contains("uses: ./.github/workflows/_build-and-test.yml", develop, StringComparison.Ordinal);
        Assert.Contains("provenance: mode=max", develop, StringComparison.Ordinal);
        Assert.Contains("sbom: true", develop, StringComparison.Ordinal);
        Assert.Contains("steps.build.outputs.digest", develop, StringComparison.Ordinal);
        Assert.Contains("severity: HIGH,CRITICAL", develop, StringComparison.Ordinal);
        Assert.Contains("dev-${{ steps.version.outputs.short_sha }}", develop, StringComparison.Ordinal);

        foreach (var workflowName in new[] { "ci-staging.yml", "ci-main.yml" })
        {
            var release = ReadRepositoryFile(".github", "workflows", workflowName);
            Assert.Contains("uses: ./.github/workflows/_build-and-test.yml", release, StringComparison.Ordinal);
            Assert.Contains("docker buildx imagetools create", release, StringComparison.Ordinal);
            Assert.DoesNotContain("docker build ", release, StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// PR validation must exercise the actual production image with its real infrastructure dependencies.
    /// </summary>
    [Fact]
    public void PrWorkflow_RunsProductionImageSmokeWithDiagnosticsAndCleanup()
    {
        var workflow = ReadRepositoryFile(".github", "workflows", "pr-validation.yml");

        Assert.Contains("Run production image startup and liveness smoke", workflow, StringComparison.Ordinal);
        Assert.Contains("postgres:18-alpine", workflow, StringComparison.Ordinal);
        Assert.Contains("redis:7-alpine", workflow, StringComparison.Ordinal);
        Assert.Contains(
            "rabbitmq:4-management-alpine@sha256:0753b75ce99094c385483d89449d532a0544fb85e4942a478b21cc497ab66d33",
            workflow,
            StringComparison.Ordinal);
        Assert.Contains("--env HOME=/tmp", workflow, StringComparison.Ordinal);
        Assert.Contains("--env RABBITMQ_MNESIA_BASE=/tmp/rabbitmq/mnesia", workflow, StringComparison.Ordinal);
        Assert.Contains(
            "http://127.0.0.1:15672/api/health/checks/local-alarms",
            workflow,
            StringComparison.Ordinal);
        Assert.Contains("openssl rand -hex 24", workflow, StringComparison.Ordinal);
        Assert.Contains("RABBITMQ_DEFAULT_USER=\"$rabbit_user\"", workflow, StringComparison.Ordinal);
        Assert.Contains("RABBITMQ_DEFAULT_PASS=\"$rabbit_password\"", workflow, StringComparison.Ordinal);
        Assert.Contains("Authorization: Basic $rabbit_auth", workflow, StringComparison.Ordinal);
        Assert.DoesNotContain(string.Concat("Z3Vlc3", "Q6Z3Vlc3Q="), workflow, StringComparison.Ordinal);
        Assert.DoesNotContain(string.Concat("guest", ":", "guest"), workflow, StringComparison.Ordinal);
        Assert.DoesNotContain(string.Concat("--password=", "guest"), workflow, StringComparison.Ordinal);
        Assert.DoesNotContain("--tmpfs /var/lib/rabbitmq", workflow, StringComparison.Ordinal);
        Assert.DoesNotContain("RABBITMQ_ERLANG_COOKIE", workflow, StringComparison.Ordinal);
        Assert.DoesNotContain("--entrypoint sh", workflow, StringComparison.Ordinal);
        Assert.DoesNotContain("chown -R rabbitmq:rabbitmq", workflow, StringComparison.Ordinal);
        Assert.Contains("/lifecycle/liveness", workflow, StringComparison.Ordinal);
        Assert.Contains("trap cleanup EXIT", workflow, StringComparison.Ordinal);
        Assert.Contains("docker logs", workflow, StringComparison.Ordinal);
        Assert.Contains("^[0-9]+$", workflow, StringComparison.Ordinal);
        Assert.Contains("State.Running", workflow, StringComparison.Ordinal);
    }

    /// <summary>
    /// A promotable attestation must only be emitted after the exact digest passes the severity gate.
    /// </summary>
    [Fact]
    public void ReleaseWorkflows_AttestOnlyAfterExactDigestSeverityGate()
    {
        var develop = ReadRepositoryFile(".github", "workflows", "ci-develop.yml");
        var staging = ReadRepositoryFile(".github", "workflows", "ci-staging.yml");
        var production = ReadRepositoryFile(".github", "workflows", "ci-main.yml");

        Assert.True(
            develop.IndexOf("Scan published digest for high-severity vulnerabilities", StringComparison.Ordinal) <
            develop.IndexOf("Generate build provenance attestation", StringComparison.Ordinal));
        Assert.True(
            staging.IndexOf("Scan verified development digest for high-severity vulnerabilities", StringComparison.Ordinal) <
            staging.IndexOf("Attest approved staging release identity", StringComparison.Ordinal));
        Assert.Contains(
            "gh attestation verify \"oci://$SOURCE_IMAGE@$source_digest\"",
            production,
            StringComparison.Ordinal);
        Assert.Contains(
            "Maliev.LifecycleService/.github/workflows/ci-staging.yml",
            production,
            StringComparison.Ordinal);
    }

    /// <summary>
    /// Generated WIF and local credentials must never enter the image context or BuildKit cache.
    /// </summary>
    [Fact]
    public void DockerContext_ExcludesGeneratedAndLocalCredentialFiles()
    {
        var dockerIgnore = ReadRepositoryFile(".dockerignore");

        Assert.Contains("gha-creds-*.json", dockerIgnore, StringComparison.Ordinal);
        Assert.Contains(".env\n", dockerIgnore, StringComparison.Ordinal);
        Assert.Contains(".env.*", dockerIgnore, StringComparison.Ordinal);
        Assert.Contains("*.pem", dockerIgnore, StringComparison.Ordinal);
        Assert.Contains("*.key", dockerIgnore, StringComparison.Ordinal);
        Assert.Contains("*.pfx", dockerIgnore, StringComparison.Ordinal);
        Assert.Contains("*.p12", dockerIgnore, StringComparison.Ordinal);
    }

    /// <summary>
    /// Restored packages must persist in the build layer rather than a mutable cache mount.
    /// </summary>
    [Fact]
    public void DockerPackageRestore_PersistsPackagesAcrossBuildInstructions()
    {
        var dockerfile = ReadRepositoryFile("Maliev.LifecycleService.Api", "Dockerfile");

        Assert.DoesNotContain("--mount=type=cache", dockerfile, StringComparison.Ordinal);
        Assert.Contains("--no-restore", dockerfile, StringComparison.Ordinal);
    }

    /// <summary>
    /// SBOM evidence must fail closed if the scanner does not produce the requested file.
    /// </summary>
    [Theory]
    [InlineData("pr-validation.yml")]
    [InlineData("ci-develop.yml")]
    public void SbomUpload_UsesValidFailClosedMissingFilePolicy(string workflowName)
    {
        var workflow = ReadRepositoryFile(".github", "workflows", workflowName);

        Assert.Contains("if-no-files-found: error", workflow, StringComparison.Ordinal);
        Assert.DoesNotContain("if-no-files-found: erro\n", workflow, StringComparison.Ordinal);
    }

    /// <summary>
    /// All third-party workflow actions must be pinned to immutable commit SHAs.
    /// </summary>
    [Fact]
    public void Workflows_PinAllThirdPartyActionsToCommitShas()
    {
        var workflowsDirectory = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            ".github",
            "workflows"));

        foreach (var workflowPath in Directory.EnumerateFiles(workflowsDirectory, "*.yml"))
        {
            foreach (var line in File.ReadLines(workflowPath))
            {
                var trimmed = line.Trim();
                if (!trimmed.StartsWith("uses:", StringComparison.Ordinal) || trimmed.Contains("./", StringComparison.Ordinal))
                {
                    continue;
                }

                var reference = trimmed["uses:".Length..].Trim();
                var atIndex = reference.LastIndexOf('@');
                Assert.True(atIndex > 0, $"Action is not pinned in {workflowPath}: {trimmed}");
                var version = reference[(atIndex + 1)..];
                Assert.Matches("^[0-9a-f]{40}$", version);
            }
        }
    }

    /// <summary>
    /// Supplies image workflows to shared policy assertions.
    /// </summary>
    public static TheoryData<string> GetImageWorkflows()
    {
        var data = new TheoryData<string>();
        foreach (var workflow in ImageWorkflows)
        {
            data.Add(workflow);
        }

        return data;
    }

    private static string ReadRepositoryFile(params string[] segments)
    {
        var path = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            Path.Combine(segments)));

        Assert.True(File.Exists(path), $"Could not find source file: {path}");
        return File.ReadAllText(path).Replace("\r\n", "\n", StringComparison.Ordinal);
    }
}
