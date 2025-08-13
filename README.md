# BKU

Modern bir ASP.NET Core + Flutter quiz uygulaması.
Backend: REST API + SignalR ile gerçek zamanlı yayın, EF Core + SQL Server, Serilog ile loglama.
Frontend: Flutter (+ Riverpod) ile modern soru–cevap arayüzü ve canlı güncellemeler.

Özellikler
✅ Soru & şık yönetimi (Question/Answer)

✅ Cevap işaretleme ve doğruluk kontrolü (UserAnswer)

✅ Gerçek zamanlı yayın (SignalR) — yeni soru eklendiğinde istemcilerde anında belirme

✅ Modern Flutter UI (Material 3, Google Fonts)

✅ Skor sayfası (kaç doğru/yanlış/boş)

✅ Serilog ile dosyaya ve konsola detaylı log

Not: JWT kimlik doğrulama opsiyonel. Şu anki kurulumda kapalı (Hub’ta [Authorize] yok, Program.cs’de AddAuthentication yorumlu).

┌─────────────────────────────────────┐
│            Flutter (App)            │
│  - http: REST /api/*                │
│  - signalr_core: /hubs/quiz         │
│  - Riverpod state mgmt.             │
└───────────────▲───────────▲─────────┘
                │           │
         REST (JSON)   SignalR Events
                │           │
┌───────────────┴───────────┴─────────┐
│          ASP.NET Core API            │
│ Controllers:                         │
│  - QuestionController                │
│  - AnswerController                  │
│  - UserAnswersController (submit)    │
│ Hubs: QuizHub (SignalR)              │
│ Repos + EF Core (SQL Server)         │
│ Serilog Logs: /logs/log-*.txt        │
└──────────────────────────────────────┘
A/B/C/D etiketi sadece UI tarafında; sunucuya her zaman AnswerId gönderilir.

Klasör Yapısı
Backend
Kopyala
Düzenle
BKU/
├─ Controllers/
│  ├─ QuestionController.cs
│  ├─ AnswerController.cs
│  └─ UserAnswersController.cs
├─ Hubs/
│  └─ QuizHub.cs
├─ Data/
│  └─ ApplicationDbContext.cs
├─ Models/
│  ├─ Kullanicilar.cs
│  ├─ Question.cs
│  ├─ Answer.cs
│  └─ UserAnswer.cs
├─ Repository/
│  ├─ Interfaces/ (IQuestionRepository, IAnswerRepository, IKullanicilarRepository, IUserAnswerRepository)
│  └─ ... (implementations)
├─ appsettings.json
└─ Program.cs
Flutter
arduino
Kopyala
Düzenle
lib/
├─ core/
│  ├─ config.dart
│  └─ http_client.dart
├─ data/
│  ├─ models/ (question.dart, answer.dart)
│  └─ services/ (quiz_api.dart, signalr_service.dart)
├─ features/quiz/
│  ├─ quiz_controller.dart
│  ├─ quiz_providers.dart
│  ├─ quiz_screen.dart
│  ├─ result_screen.dart
│  └─ widgets/ (answer_chip.dart, question_card.dart, progress_bar.dart)
├─ app_theme.dart
└─ main.dart
Kullanılan Teknolojiler
Backend

ASP.NET Core Web API

Entity Framework Core (SQL Server)

SignalR (Gerçek zamanlı)

Serilog (loglama)

(Opsiyonel) JWT Bearer

Frontend

Flutter (Dart)

flutter_riverpod (durum yönetimi)

http (REST)

signalr_core (SignalR istemcisi)

google_fonts (UI)

Hızlı Başlangıç
1) Gereksinimler
.NET SDK 8.x (7.x de olur)

SQL Server / SQL Express

Flutter SDK 3.5.x

Android/iOS araçları (Android Studio veya Xcode)

2) Backend – Kurulum & Çalıştırma
appsettings.json

json
Kopyala
Düzenle
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=CSNBAHMETESER\\SQLEXPRESS01;Database=BKUDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  },
  "AllowedHosts": "*"
}
Program.cs – kritik notlar

Geliştirmede HTTP kullanıyorsanız app.UseHttpsRedirection() kapalı olabilir.

Emülatör/cihazdan erişim için:

csharp
Kopyala
Düzenle
// builder.WebHost.UseUrls("http://0.0.0.0:5058", "http://localhost:5058");
builder.WebHost.UseUrls("http://172.22.18.61:5058"); // kendi IP'nize göre
EF Core Migration

bash
Kopyala
Düzenle
dotnet tool install --global dotnet-ef       # ilk kez
dotnet ef migrations add Init
dotnet ef database update
Alternatif (Visual Studio Package Manager Console):
Add-Migration Init → Update-Database

Çalıştırma

bash
Kopyala
Düzenle
dotnet restore
dotnet run
Loglar

Konsol + ./logs/log-*.txt

3) Flutter – Kurulum & Çalıştırma
pubspec.yaml (özet)

yaml
Kopyala
Düzenle
dependencies:
  flutter:
    sdk: flutter
  http: ^1.2.2
  flutter_riverpod: ^2.5.1
  signalr_core: ^1.1.2
  google_fonts: ^6.2.1
Android Manifest (HTTP için)

xml
Kopyala
Düzenle
<application
    android:usesCleartextTraffic="true"
    ... />
lib/core/config.dart

dart
Kopyala
Düzenle
// Android emülatör: host makine = 10.0.2.2
const baseUrl = 'http://10.0.2.2:5058';
// Fiziksel cihaz: bilgisayarınızın LAN IP'si ve portu
// const baseUrl = 'http://192.168.1.34:5058';

const apiBase = '$baseUrl/api';
const hubUrl  = '$baseUrl/hubs/quiz';

// JWT yoksa null bırak
String? get jwtToken => null;
Çalıştırma

bash
Kopyala
Düzenle
flutter pub get
flutter run
API Uçları (Örnekler)
Soru listesi

bash
Kopyala
Düzenle
GET /api/Question
200 OK
[
  {
    "id": 12,
    "text": "HTTP nedir?",
    "answers": [
      {"id": 101, "text": "...", "isCorrect": true, "questionId": 12},
      ...
    ]
  }
]
Soru detayı

bash
Kopyala
Düzenle
GET /api/Question/{id}
Cevap gönder

bash
Kopyala
Düzenle
POST /api/UserAnswers/submit
Content-Type: application/json

{
  "questionId": 12,
  "answerId": 101
}

200 OK
{ "correct": true, "id": 5, "userId": null, "questionId": 12, "answerId": 101 }
UI’daki A/B/C/D sadece göstergedir; answerId sunucuya gönderilir.

SignalR (Gerçek Zamanlı)
Hub

Endpoint: /hubs/quiz

Sınıf: QuizHub

(Şu an) [Authorize] YOK → token gerekmiyor

Yayınlanan Event’ler (önerilen)

"QuestionAdded": admin yeni soru ekleyince

"UserAnswered" : kullanıcı cevap gönderince (opsiyon)

Flutter bağlantı

dart
Kopyala
Düzenle
final hub = HubConnectionBuilder()
  .withUrl(hubUrl, HttpConnectionOptions(
    transport: HttpTransportType.webSockets,
    accessTokenFactory: jwtToken == null ? null : () async => jwtToken!,
  ))
  .withAutomaticReconnect()
  .build();

hub.on('QuestionAdded', (args) { /* ... */ });
await hub.start();
Geliştirme İpuçları
Emülatör → backend erişimi için 10.0.2.2 kullan.

Serilog ile backend konsoldaki hataları yakından takip et.

Flutter’da geniş log için flutter run -v.

EF’de tablo adı çakışması (örn. There is already an object named ...):

Gerekiyorsa Drop-Database (PM Console) veya yeni migration oluşturup düzelt.

Sorun Giderme
SignalR negotiate 500

Hub’ta [Authorize] varken AddAuthentication yok → kaldır veya auth ekle.

HTTPS yönlendirme açık ve Flutter http kullanıyor → app.UseHttpsRedirection() devde kapatın ya da https kullanın.

CORS engeli (tarayıcı için). Flutter native’de genelde sorun olmaz.

Flutter spinner dönüyor (GET bekliyor)

baseUrl doğru mu? Emülatörde 10.0.2.2.

Backend UseUrls ile doğru arayüzde mi dinliyor?

Android Manifest’te usesCleartextTraffic="true" var mı?

EF Migration Hatası

Eski migration’lar DB ile uyumsuz → Remove-Migration, Update-Database, veya Drop-Database + Add-Migration + Update-Database.

Yol Haritası (Opsiyonel)
🔐 JWT’yi devreye alma (Hub + API)

👥 Kullanıcıya bağlı skor/puan geçmişi

🧪 Unit/Integration testler

🐳 Docker Compose (api + sqlserver)

📊 Admin panel (soru yönetimi, canlı skor)

