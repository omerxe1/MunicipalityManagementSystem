# FAZ 4: Deployment ve Backup Güvenlik Notları

## ⚠️ ÖNEMLİ GÜVENLİK AYARLARI

### 1. Production Ortamı Ayarları

#### appsettings.Production.json Kontrol Listesi:
- [ ] `ConnectionStrings:KocaaliContext` - Production veritabanı bağlantı string'i güvenli mi?
- [ ] `Security:AllowedIPs` - Admin panel için IP kısıtlaması aktif mi?
- [ ] `Security:EnableIPRestriction` - `true` olarak ayarlanmalı
- [ ] `Security:RateLimit:MaxRequestsPerMinute` - Rate limit ayarları uygun mu?
- [ ] `ReCaptcha:EnableReCaptcha` - Production'da `true` olmalı
- [ ] `ReCaptcha:SiteKey` ve `SecretKey` - Geçerli reCAPTCHA anahtarları var mı?

#### Environment Variables:
```bash
# Production'da mutlaka ayarlanmalı:
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=https://yourdomain.com
```

### 2. HTTPS ve SSL Sertifikası

- [ ] SSL sertifikası geçerli ve güncel mi?
- [ ] HTTPS zorunlu yönlendirme aktif mi? (`UseHttpsRedirection`)
- [ ] HSTS header'ları doğru yapılandırılmış mı?
- [ ] Cookie SecurePolicy `Always` olarak ayarlanmış mı?

### 3. Veritabanı Güvenliği

#### Backup Öncesi Kontroller:
- [ ] Veritabanı yedekleme script'i güvenli bir yerde saklanıyor mu?
- [ ] Backup dosyaları şifrelenmiş mi?
- [ ] Backup dosyalarına erişim kısıtlı mı?
- [ ] Connection string'de şifre güvenli mi? (appsettings.Production.json'da)

#### Backup Sırasında:
```sql
-- Veritabanı backup komutu (SQL Server)
BACKUP DATABASE KocaaliDB 
TO DISK = 'C:\Backups\KocaaliDB_YYYYMMDD_HHMMSS.bak'
WITH FORMAT, COMPRESSION, ENCRYPTION;
```

#### Backup Sonrası:
- [ ] Backup dosyası doğrulanmış mı?
- [ ] Backup dosyası güvenli bir yere kopyalanmış mı?
- [ ] Eski backup dosyaları düzenli olarak temizleniyor mu?

### 4. Dosya Yükleme Güvenliği

- [ ] `wwwroot/uploads` klasörü dışarıdan erişilebilir mi? (Sadece gerekli dosyalar)
- [ ] Yüklenen dosyaların boyut limitleri uygun mu?
- [ ] Dosya uzantı ve MIME type kontrolleri aktif mi?
- [ ] Magic bytes kontrolü çalışıyor mu?

### 5. Loglama ve İzleme

#### Log Dosyaları:
- [ ] Log dosyaları güvenli bir yerde saklanıyor mu?
- [ ] Log dosyalarına erişim kısıtlı mı?
- [ ] Log dosyaları düzenli olarak arşivleniyor mu?
- [ ] Hassas bilgiler (şifreler, token'lar) loglanmıyor mu?

#### İzleme:
- [ ] Güvenlik olayları (brute force, rate limit) izleniyor mu?
- [ ] Admin panel işlemleri loglanıyor mu?
- [ ] Hata logları düzenli olarak kontrol ediliyor mu?

### 6. Session ve Cookie Güvenliği

- [ ] Session timeout süresi uygun mu? (15 dakika)
- [ ] Cookie HttpOnly aktif mi?
- [ ] Cookie SameSite=Strict ayarlanmış mı?
- [ ] Cookie SecurePolicy Production'da `Always` mi?

### 7. Rate Limiting

- [ ] Rate limiting aktif mi?
- [ ] Rate limit değerleri uygun mu? (Dakika: 60, Saat: 1000)
- [ ] Admin panel için daha sıkı limitler uygulanıyor mu?

### 8. Admin Panel Güvenliği

- [ ] Admin panel URL'i gizli mi? (`/yonetim-portal`)
- [ ] IP kısıtlaması aktif mi?
- [ ] Brute force koruması çalışıyor mu?
- [ ] Şifre karmaşıklığı kuralları uygulanıyor mu?
- [ ] BasAdmin şifre sıfırlama yetkisi doğru çalışıyor mu?

### 9. Kod Güvenliği

#### Deployment Öncesi:
- [ ] `SeedController` production'da devre dışı mı?
- [ ] Debug modu kapalı mı?
- [ ] Detaylı hata mesajları kapatılmış mı?
- [ ] Gereksiz dosyalar (test, debug) kaldırılmış mı?

#### Kod İnceleme:
- [ ] SQL injection riski var mı? (Entity Framework kullanılıyor mu?)
- [ ] XSS koruması aktif mi? (Html.Raw kullanımları güvenli mi?)
- [ ] CSRF koruması var mı? (AntiForgeryToken)
- [ ] Input validation yapılıyor mu?

### 10. Sunucu Güvenliği

- [ ] Firewall kuralları doğru yapılandırılmış mı?
- [ ] Gereksiz portlar kapatılmış mı?
- [ ] İşletim sistemi güncellemeleri yapılmış mı?
- [ ] Antivirus/antimalware yazılımı aktif mi?

### 11. Yedekleme Stratejisi

#### Günlük Yedekleme:
- [ ] Veritabanı günlük yedekleniyor mu?
- [ ] Yedek dosyaları şifrelenmiş mi?
- [ ] Yedek dosyaları farklı bir lokasyonda saklanıyor mu?

#### Haftalık Yedekleme:
- [ ] Tam sistem yedeği alınıyor mu?
- [ ] Yedek dosyaları test ediliyor mu?

#### Aylık Yedekleme:
- [ ] Arşiv yedekleri oluşturuluyor mu?
- [ ] Yedek dosyaları uzun süreli saklanıyor mu?

### 12. Acil Durum Planı

- [ ] Veri kaybı durumunda geri yükleme prosedürü hazır mı?
- [ ] Güvenlik ihlali durumunda yapılacaklar belirlenmiş mi?
- [ ] İletişim bilgileri güncel mi?
- [ ] Yedekleme dosyalarına erişim test edilmiş mi?

## 🔒 Güvenlik Kontrol Listesi (Her Deployment Öncesi)

1. ✅ Production ayarları kontrol edildi
2. ✅ HTTPS ve SSL sertifikası geçerli
3. ✅ Veritabanı bağlantı string'i güvenli
4. ✅ IP kısıtlaması aktif
5. ✅ Rate limiting aktif
6. ✅ Brute force koruması aktif
7. ✅ Loglama sistemi çalışıyor
8. ✅ Cookie güvenlik ayarları doğru
9. ✅ Session timeout ayarlanmış
10. ✅ SeedController devre dışı
11. ✅ Debug modu kapalı
12. ✅ Detaylı hata mesajları kapatılmış

## 📝 Notlar

- **ÖNEMLİ**: Production'da `SeedController` kullanılmamalıdır. Test amaçlıdır.
- **ÖNEMLİ**: `appsettings.Production.json` dosyası asla version control'e commit edilmemelidir.
- **ÖNEMLİ**: Backup dosyaları şifrelenmeli ve güvenli bir yerde saklanmalıdır.
- **ÖNEMLİ**: Log dosyalarında hassas bilgiler (şifreler, token'lar) bulunmamalıdır.

## 🚨 Güvenlik İhlali Durumunda

1. Hemen admin paneline erişimi kısıtla
2. Şüpheli IP'leri engelle
3. Tüm şifreleri değiştir
4. Log dosyalarını incele
5. Güvenlik açığını tespit et ve kapat
6. Gerekirse sistemi geçici olarak kapat
7. İlgili yetkililere bildir

---

**Son Güncelleme**: FAZ 4 Güvenlik Uygulaması
**Versiyon**: 1.0



