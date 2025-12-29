\# 🛒 E-Ticaret Web Uygulaması



\*\*ASP.NET Core MVC\*\* ve \*\*Entity Framework Core\*\* ile geliştirilmiş tam özellikli bir e-ticaret platformu.



\[🇬🇧 Click here for English README](README.md)



---



\## 🚀 Özellikler



\### 👥 Kullanıcı Yönetimi

\- Kullanıcı kayıt ve giriş sistemi

\- Profil yönetimi

\- Admin ve satıcı rol desteği

\- Oturum tabanlı kimlik doğrulama



\### 🛍️ Ürün Yönetimi

\- Ürün CRUD işlemleri

\- Kategori yönetimi

\- Resim yükleme desteği

\- Stok takibi

\- Aktif/pasif ürün durumu

\- Geri dönüşümlü silme (soft delete)



\### 🛒 Alışveriş Sepeti

\- Ürün ekleme/çıkarma

\- Miktar yönetimi

\- Anlık toplam hesaplama

\- Kalıcı sepet saklama



\### 📦 Sipariş Yönetimi

\- Sipariş oluşturma ve takip

\- Sipariş durum yönetimi

\- Kargo bilgileri

\- Sipariş geçmişi



\### 🔧 Yönetim Paneli

\- Ürün yönetim paneli

\- Kategori yönetimi

\- İstatistik ve analitik

\- Silinen öğeleri geri yükleme

\- Düşük stok uyarıları



\### 📚 API Dokümantasyonu

\- Swagger/OpenAPI ile RESTful API

\- İnteraktif API test arayüzü

\- Kapsamlı endpoint dokümantasyonu



---



\## 🛠️ Kullanılan Teknolojiler



\### Backend

\- \*\*Framework\*\*: ASP.NET Core 8.0 MVC

\- \*\*ORM\*\*: Entity Framework Core 8.0

\- \*\*Veritabanı\*\*: SQL Server

\- \*\*API Dokümantasyon\*\*: Swagger/Swashbuckle



\### Frontend

\- \*\*CSS Framework\*\*: Tailwind CSS

\- \*\*İkonlar\*\*: Font Awesome

\- \*\*Yazı Tipleri\*\*: Google Fonts (Inter)



\### Mimari

\- \*\*Pattern\*\*: MVC (Model-View-Controller)

\- \*\*Veri Katmanı\*\*: Repository Pattern

\- \*\*İş Mantığı\*\*: Service Layer

\- \*\*Kimlik Doğrulama\*\*: Session-based



---



\## 📋 Gereksinimler



\- \[.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

\- \[SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (Express veya LocalDB)

\- \[Visual Studio 2022](https://visualstudio.microsoft.com/) veya \[VS Code](https://code.visualstudio.com/)

\- \[Git](https://git-scm.com/)



---



\## ⚙️ Kurulum



\### 1. Projeyi Klonlayın



```bash

git clone https://github.com/Mamyy25/mini\_eticaretuygulamasi.git

cd mini\_eticaretuygulamasi

```



\### 2. Uygulama Ayarlarını Yapılandırın



Örnek ayar dosyasını kopyalayın ve kendi yapılandırmanızla güncelleyin:



```bash

cd ECommerce.Web

copy appsettings.example.json appsettings.json

```



Ardından `ECommerce.Web/appsettings.json` dosyasını açın ve bağlantı dizesini güncelleyin:



```json

{

&nbsp; "ConnectionStrings": {

&nbsp;   "DefaultConnection": "Server=SUNUCU\_ADINIZ;Database=ECommerceDB;Trusted\_Connection=True;TrustServerCertificate=True;"

&nbsp; }

}

```



\*\*⚠️ Önemli\*\*: Gerçek bağlantı dizelerini içeren `appsettings.json` dosyasını asla Git'e commit etmeyin!



\### 3. Veritabanı Migration'larını Uygulayın



```bash

cd ECommerce.Data

dotnet ef database update --startup-project ../ECommerce.Web

```



Veya Visual Studio'da Package Manager Console'da:



```powershell

Update-Database

```



\### 4. Uygulamayı Çalıştırın



```bash

cd ECommerce.Web

dotnet run

```



Veya Visual Studio'da \*\*F5\*\* tuşuna basın.



\### 5. Uygulamaya Erişin



\- \*\*Web Sitesi\*\*: `http://localhost:5133`

\- \*\*Swagger API\*\*: `http://localhost:5133/swagger`



---



\## 📁 Proje Yapısı



```

ECommerce/

├── ECommerce.Web/              # MVC Web Uygulaması

│   ├── Controllers/            # MVC Controller'lar

│   │   ├── API/               # API Controller'lar

│   │   ├── HomeController.cs

│   │   ├── ProductController.cs

│   │   └── ...

│   ├── Views/                 # Razor View'lar

│   ├── wwwroot/               # Statik dosyalar

│   └── Program.cs             # Uygulama giriş noktası

│

├── ECommerce.Data/            # Veri Erişim Katmanı

│   ├── ApplicationDbContext.cs

│   └── Migrations/

│

├── ECommerce.Models/          # Domain Modelleri

│   ├── Product.cs

│   ├── Category.cs

│   ├── User.cs

│   └── ...

│

└── ECommerce.Business/        # İş Mantığı Katmanı

&nbsp;   └── Services/

```



---



\## 🔐 Varsayılan Admin Hesabı



Migration'ları çalıştırdıktan sonra, veritabanında manuel olarak veya kayıt sayfası üzerinden admin yetkili bir hesap oluşturabilirsiniz.



---



\## 🌐 API Endpoint'leri



\### Ürün API'ları



| Method | Endpoint | Açıklama |

|--------|----------|----------|

| GET | `/api/ProductsApi` | Tüm ürünleri getir |

| GET | `/api/ProductsApi/{id}` | ID'ye göre ürün getir |

| GET | `/api/ProductsApi/category/{categoryId}` | Kategoriye göre ürünleri getir |

| GET | `/api/ProductsApi/search?searchTerm={terim}` | Ürün ara |

| POST | `/api/ProductsApi` | Yeni ürün oluştur |

| PUT | `/api/ProductsApi/{id}` | Ürün güncelle |

| DELETE | `/api/ProductsApi/{id}` | Ürün sil (soft delete) |

| GET | `/api/ProductsApi/{id}/stock` | Stok bilgisi getir |



\### Kategori API'ları



| Method | Endpoint | Açıklama |

|--------|----------|----------|

| GET | `/api/CategoriesApi` | Tüm kategorileri getir |

| GET | `/api/CategoriesApi/{id}` | ID'ye göre kategori getir |

| POST | `/api/CategoriesApi` | Yeni kategori oluştur |

| PUT | `/api/CategoriesApi/{id}` | Kategori güncelle |

| DELETE | `/api/CategoriesApi/{id}` | Kategori sil (soft delete) |

| GET | `/api/CategoriesApi/{id}/product-count` | Kategorideki ürün sayısını getir |



\*\*Tam API dokümantasyonu\*\*: `http://localhost:5133/swagger`



---



\## 🎨 Özellik Gösterimi



\### Yönetim Paneli

\- Gerçek zamanlı istatistik panosu

\- Dropdown menü ile ürün ve kategori yönetimi

\- Stok uyarıları ile envanter takibi

\- Silinen öğeleri geri yükleme

\- Toplu işlem desteği



\### Kullanıcı Özellikleri

\- Tailwind CSS ile responsive tasarım

\- Temiz ve modern minimalist arayüz

\- Gerçek zamanlı güncellenen alışveriş sepeti

\- Sipariş takibi ve geçmişi

\- Oturum tabanlı sepet kalıcılığı



\### API

\- Uygun HTTP metodlarıyla RESTful tasarım

\- İnteraktif Swagger/OpenAPI dokümantasyonu

\- Döngüsel referans yönetimi ile JSON yanıtlar

\- Kapsamlı hata yönetimi

\- Bearer token kimlik doğrulama desteği (uygulamaya hazır)



---



\## 🐛 Sorun Giderme



\### Veritabanı Bağlantı Sorunları



Bağlantı hatası alıyorsanız:



1\. SQL Server'ın çalıştığını kontrol edin

2\. `appsettings.json` içindeki bağlantı dizesini güncelleyin

3\. Migration'ları çalıştırın: `dotnet ef database update`



\### Port Zaten Kullanımda



`ECommerce.Web/Properties/launchSettings.json` dosyasında portu değiştirin



\### NuGet Paket Hataları



```bash

dotnet restore

dotnet clean

dotnet build

```



\### API'de Döngüsel Referans Hataları



Program.cs'de `ReferenceHandler.IgnoreCycles` ile zaten yönetiliyor



---



\## 🤝 Katkıda Bulunma



1\. Projeyi fork edin

2\. Feature branch'i oluşturun (`git checkout -b feature/HarikaOzellik`)

3\. Değişikliklerinizi commit edin (`git commit -m 'Harika özellik eklendi'`)

4\. Branch'inizi push edin (`git push origin feature/HarikaOzellik`)

5\. Pull Request açın



---



\## 📝 Lisans

**Telif Hakkı © 2025 Mamyy25. Tüm hakları saklıdır.**

Bu proje özel mülkiyettedir ve gizlidir. Bu yazılımın izinsiz kopyalanması, 
dağıtılması, değiştirilmesi veya kullanılması, sahibinden açık yazılı izin 
alınmaksızın kesinlikle yasaktır.

Lisans sorgulamaları için lütfen iletişime geçin: [mekkoseoglu@gmail.com]

Daha fazla detay için [LICENSE](LICENSE) dosyasına bakın.

---



\## 👨‍💻 Geliştirici



\*\*Mamyy25\*\*

\- GitHub: \[@Mamyy25](https://github.com/Mamyy25)

\- Proje Repository: \[mini\_eticaretuygulamasi](https://github.com/Mamyy25/mini\_eticaretuygulamasi)



---



\## 🙏 Teşekkürler



\- ASP.NET Core Ekibi

\- Entity Framework Core Ekibi

\- Tailwind CSS

\- Font Awesome

\- Swagger/OpenAPI

\- Google Fonts



---



\## 🔒 Güvenlik Notları



\- Bağlantı dizeleri `.gitignore` ile Git'ten hariç tutulmuştur

\- Yapılandırma için şablon olarak `appsettings.example.json` kullanın

\- Yapılandırılabilir zaman aşımı ile oturum tabanlı kimlik doğrulama

\- Entity Framework parametrelendirme ile SQL injection koruması

\- Razor view encoding ile XSS koruması



---





