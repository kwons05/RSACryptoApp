using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using RSACryptoApp.Models;
using System.Diagnostics;
using System.IO;

namespace RSACryptoApp
{
    public class RSACryptoService
    {
        public string WorkDirectory = string.Empty;

        public string FILE_KEY  = "private_key.pem";
        public string FILE_CERT = "certificate.crt";

        public Action<string> WriteLog;


        public RSACryptoService()
        {
            WorkDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "result");
            if (!Directory.Exists(WorkDirectory)) {
                Directory.CreateDirectory(WorkDirectory);
            }
        }

        public void Create(Parameter param)
        {
            // RSA 키 쌍 생성
            var random = new SecureRandom();
            var keyGenerationParameters = new KeyGenerationParameters(random, 2048);

            RsaKeyPairGenerator generator = new RsaKeyPairGenerator();
            generator.Init(keyGenerationParameters);

            var keyPair = generator.GenerateKeyPair();

            // X.509 인증서 생성
            var certificate = GenerateSelfSignedCertificate(keyPair, param);

            // 개인 키 저장
            using (var writer = new StreamWriter(Path.Combine(WorkDirectory, FILE_KEY))) {
                var pemWriter = new PemWriter(writer);
                pemWriter.WriteObject(keyPair.Private);
            }
            using (var stringWriter = new StringWriter()) {
                var pemWriter = new PemWriter(stringWriter);
                pemWriter.WriteObject(keyPair.Private);
                WriteLog?.Invoke(stringWriter.ToString());
            }

            // 인증서 저장 (PEM 형식으로 저장)
            using (var writer = new StreamWriter(Path.Combine(WorkDirectory, FILE_CERT))) {
                var pemWriter = new PemWriter(writer);
                pemWriter.WriteObject(certificate);
            }
            using (var stringWriter = new StringWriter()) {
                var pemWriter = new PemWriter(stringWriter);
                pemWriter.WriteObject(certificate);
                WriteLog?.Invoke(stringWriter.ToString());
            }
        }
        /// <summary>
        /// Self Signed Certificate
        /// </summary>
        /// <param name="keyPair"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public X509Certificate GenerateSelfSignedCertificate(AsymmetricCipherKeyPair keyPair, Parameter param)
        {
            var certGen = new X509V3CertificateGenerator();

            var serialNumber = BigInteger.ProbablePrime(120, new Random());
            certGen.SetSerialNumber(serialNumber);

            var attrs = new Dictionary<DerObjectIdentifier, string>();

            // 인증서 정보 설정
            attrs.Add(X509Name.CN, param.Domain);
            attrs.Add(X509Name.C, param.Country);
            attrs.Add(X509Name.ST, param.State);
            attrs.Add(X509Name.L, param.Locality);
            attrs.Add(X509Name.O, param.Organization);

            var subjectDN = new X509Name(attrs.Keys.ToArray(), attrs);

            // 발급자 정보 설정
            certGen.SetIssuerDN(subjectDN);
            // 주체 정보 설정
            certGen.SetSubjectDN(subjectDN);

            certGen.SetNotBefore(param.StartDate);  // 유효 시작일
            certGen.SetNotAfter(param.EndDate);     // 유효 종료일

            certGen.SetPublicKey(keyPair.Public);

            // 서명 알고리즘 설정
            var signatureFactory = new Asn1SignatureFactory("SHA256WithRSA", keyPair.Private);

            return certGen.Generate(signatureFactory);
        }
        /// <summary>
        /// 파일 서명
        /// </summary>
        /// <param name="filePath"></param>
        public void Export(string filePath)
        {

            string privatePath = Path.Combine(WorkDirectory, FILE_KEY);
            string certPath = Path.Combine(WorkDirectory, FILE_CERT);

            byte[] fileData = File.ReadAllBytes(filePath);
            // 서명 파일
            string sigFilePath = $"{Path.GetFileName(filePath)}.sig"; 

            AsymmetricKeyParameter privateKey = null;
            // 개인 키 로드
            using (var reader = File.OpenText(privatePath)) {
                var pemReader = new PemReader(reader);
                var keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
                privateKey = keyPair.Private;
            }

            // SHA-256 해시 계산 및 서명
            ISigner signer = SignerUtilities.GetSigner("SHA256withRSA");
            signer.Init(true, privateKey);
            signer.BlockUpdate(fileData, 0, fileData.Length);

            byte[] signature = signer.GenerateSignature();

            // 서명 저장
            File.WriteAllBytes(Path.Combine(WorkDirectory, sigFilePath), signature);


            // 서명 검증
            byte[] certData = File.ReadAllBytes(certPath);

            // 공개 키 로드
            X509Certificate cert = null;
            using (var reader = File.OpenText(certPath)) {
                var pemReader = new PemReader(reader);
                cert = (X509Certificate)pemReader.ReadObject();
            }
            AsymmetricKeyParameter publicKey = cert.GetPublicKey();

            WriteLog?.Invoke($"{cert}");

            // 서명 검증
            ISigner verify = SignerUtilities.GetSigner("SHA256withRSA");
            verify.Init(false, publicKey);
            verify.BlockUpdate(fileData, 0, fileData.Length);
            bool isValid = verify.VerifySignature(certData);

            WriteLog?.Invoke(sigFilePath);
            WriteLog?.Invoke(isValid ? "서명이 유효합니다." : "서명이 유효하지 않습니다.");

        }
    }

}
