param(
    [string]$DnsName = "localhost",
    [int]$Years = 5
)

$certsDir = Join-Path (Split-Path $PSScriptRoot -Parent) "certs"
$crtPath  = Join-Path $certsDir "intdorsys.crt"
$keyPath  = Join-Path $certsDir "intdorsys.key"

$certExists = (Test-Path $crtPath) -and (Test-Path $keyPath)
if ($certExists) {
    Write-Host "Certificate already exists: $crtPath"
    Write-Host "Delete files manually to regenerate."
    exit 0
}

if (-not (Test-Path $certsDir)) {
    New-Item -ItemType Directory -Path $certsDir -Force | Out-Null
}

Write-Host "Generating self-signed certificate for $DnsName ..."

Add-Type @"
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

public class CertGen {
    public static void Create(string certPath, string keyPath, string dnsName, int years) {
        using (var rsa = new RSACng(4096)) {
            var req = new CertificateRequest("CN=" + dnsName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            req.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
            req.CertificateExtensions.Add(new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, false));
            var san = new SubjectAlternativeNameBuilder();
            san.AddDnsName(dnsName);
            san.AddDnsName("localhost");
            req.CertificateExtensions.Add(san.Build());
            using (var cert = req.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(years))) {
                File.WriteAllText(certPath, cert.ExportCertificatePem());
                File.WriteAllText(keyPath, rsa.ExportRSAPrivateKeyPem());
            }
        }
    }
}
"@

[CertGen]::Create($crtPath, $keyPath, $DnsName, $Years)

Write-Host "Done:"
Write-Host "  CRT: $crtPath"
Write-Host "  KEY: $keyPath"
