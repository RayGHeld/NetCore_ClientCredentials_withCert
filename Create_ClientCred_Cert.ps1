# Create self-signed certificate and export pfx and cer files

# please be aware that running this script will create a new cert each time in your certificate store.  If you need to rerun it, I recommend deleting the previous certs first in the store.

$tenant_id = 'rayheld.com' # tenant id is used for the cert name / subject name and matches the .netcore code sample that goes with this script
$client_id = 'd48514a1-0d32-4a1d-8d26-f3edcad0d2a9' # client id is used for the cert name / subject name and cert password and matches the .netcore code sample that goes with this script
$FilePath = 'C:\Users\raheld\OneDrive - Microsoft\Documents\Certs\'
$StoreLocation = 'CurrentUser' # be aware that LocalMachine requires elevated privileges
$expirationYears = 1

$SubjectName = $tenant_id + '.' + $client_id
$cert_password = $client_id

$pfxFileName = $SubjectName + '.pfx'
$cerFileName = $SubjectName + '.cer'

$PfxFilePath = $FilePath + $pfxFileName
$CerFilePath = $FilePath + $cerFileName

$CertBeginDate = Get-Date
$CertExpiryDate = $CertBeginDate.AddYears($expirationYears)
$SecStringPw = ConvertTo-SecureString -String $cert_password -Force -AsPlainText 
$Cert = New-SelfSignedCertificate -DnsName $SubjectName -CertStoreLocation "cert:\$StoreLocation\My" -NotBefore $CertBeginDate -NotAfter $CertExpiryDate -KeySpec Signature
Export-PfxCertificate -cert $Cert -FilePath $PFXFilePath -Password $SecStringPw 
Export-Certificate -cert $Cert -FilePath $CerFilePath 