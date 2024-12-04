using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Signers;
using System.IO;


namespace RSACryptoApp.Service
{
    public class SignFile
    {
        public static byte[] Sign(string filePath, AsymmetricKeyParameter privateKey)
        {
            // 파일 데이터를 읽고 SHA-256 해시 생성
            byte[] fileData = File.ReadAllBytes(filePath);

            // RSA 서명 생성
            ISigner signer = new RsaDigestSigner(new Sha256Digest());
            signer.Init(true, privateKey); // 서명을 위해 개인 키 초기화
            signer.BlockUpdate(fileData, 0, fileData.Length);
            return signer.GenerateSignature();
        }

        static bool VerifyFileSignature(string filePath, byte[] signature, AsymmetricKeyParameter publicKey)
        {
            // 파일 데이터를 읽고 SHA-256 해시 생성
            byte[] fileData = File.ReadAllBytes(filePath);

            // RSA 서명 검증
            ISigner verifier = new RsaDigestSigner(new Sha256Digest());
            verifier.Init(false, publicKey); // 검증을 위해 공개 키 초기화
            verifier.BlockUpdate(fileData, 0, fileData.Length);
            return verifier.VerifySignature(signature);
        }
    }
}
