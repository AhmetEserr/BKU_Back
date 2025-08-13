# BKU

<!-- README.md (HTML + inline CSS) -->
<div align="center" style="padding:28px 16px; background:linear-gradient(135deg,#f5ecff,#f7f9ff); border-radius:18px;">
  <h1 style="margin:0; font-size:44px;">BKÜ Quiz</h1>
  <p style="margin:8px 0 18px; font-size:16px;">
    ASP.NET Core + Flutter ile gerçek zamanlı (SignalR) quiz uygulaması
  </p>

  <p>
    <img alt=".NET" src="https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white">
    <img alt="Flutter" src="https://img.shields.io/badge/Flutter-3.x-02569B?logo=flutter&logoColor=white">
    <img alt="SQL Server" src="https://img.shields.io/badge/SQL%20Server-EF%20Core-red?logo=microsoft-sql-server&logoColor=white">
    <img alt="SignalR" src="https://img.shields.io/badge/SignalR-realtime-2ea44f">
    <img alt="Serilog" src="https://img.shields.io/badge/Serilog-logging-6DB33F">
  </p>
</div>

<br>

<h2 id="icerik">📚 İçerik</h2>
<ul>
  <li><a href="#ozet">Özet</a></li>
  <li><a href="#ozellikler">Özellikler</a></li>
  <li><a href="#mimari">Mimari & Akış</a></li>
  <li><a href="#klasor">Klasör Yapısı</a></li>
  <li><a href="#kurulum">Kurulum (Backend & Flutter)</a></li>
  <li><a href="#api">REST API & SignalR</a></li>
  <li><a href="#log">Loglama</a></li>
  <li><a href="#seed">Örnek SQL (Soru & Şık ekleme)</a></li>
  <li><a href="#trouble">Sorun Giderme</a></li>
</ul>

<hr>

<h2 id="ozet">🔎 Özet</h2>
<p>
  Backend tarafında ASP.NET Core ile REST API + SignalR Hub sunulur; veriler EF Core ile SQL Server’da tutulur. 
  Flutter uygulaması REST ile soruları çekip, cevapları gönderir; yeni soru eklendiğinde SignalR üzerinden anlık bildirim alır.
  Kimlik doğrulama opsiyoneldir (bu projede varsayılan olarak kapalı).
</p>

<h2 id="ozellikler">✨ Özellikler</h2>
<ul>
  <li>Soru & Şık yönetimi (Question / Answer)</li>
  <li>Cevap işaretleme ve doğruluk kontrolü (UserAnswer)</li>
  <li>Gerçek zamanlı yayın (SignalR) – yeni soru eklendiğinde istemcilere düşer</li>
  <li>Modern Flutter arayüzü (Material 3, Google Fonts)</li>
  <li>Skor sayfası (kaç doğru / yanlış / boş)</li>
  <li>Serilog ile dosyaya ve konsola detaylı log</li>
</ul>

<h2 id="mimari">🏗️ Mimari & Akış</h2>
<pre style="background:#0b1021;color:#e6e6e6;padding:16px;border-radius:12px;overflow:auto;">
Flutter (App)
 ├─ http: REST  /api/*
 └─ signalr_core: /hubs/quiz
        ▲
        │ REST (JSON) & SignalR Events
        ▼
ASP.NET Core API
 ├─ Controllers:
 │   ├─ QuestionController   (GET/POST/PUT/DELETE)
 │   ├─ AnswerController     (GET/POST/PUT/DELETE)
 │   └─ UserAnswersController(POST submit)
 ├─ Hubs: QuizHub (SignalR)
 ├─ Repository + EF Core (SQL Server)
 └─ Serilog Logs: backend/BKU/logs/log-*.txt

Not: UI’da A/B/C/D sadece görsel etiket; sunucuya AnswerId gönderilir.
</pre>

<h2 id="klasor">🗂️ Klasör Yapısı</h2>
<pre style="background:#0b1021;color:#e6e6e6;padding:16px;border-radius:12px;overflow:auto;">
.
├─ backend/
│  └─ BKU/
│     ├─ Controllers/
│     │  ├─ QuestionController.cs
│     │  ├─ AnswerController.cs
│     │  └─ UserAnswersController.cs
│     ├─ Hubs/QuizHub.cs
│     ├─ Data/ApplicationDbContext.cs
│     ├─ Models/ (Question.cs, Answer.cs, Kullanicilar.cs, UserAnswer.cs)
│     ├─ Repository/
│     │  ├─ Interfaces/ (IQuestionRepository.cs, IAnswerRepository.cs, IKullanicilarRepository.cs, IUserAnswerRepository.cs)
│     │  ├─ QuestionRepository.cs
│     │  ├─ AnswerRepository.cs
│     │  ├─ KullanicilarRepository.cs
│     │  └─ UserAnswerRepository.cs
│     ├─ Program.cs
│     └─ appsettings.json
└─ app/
   └─ bku_v1/
      ├─ lib/
      │  ├─ core/ (config.dart, http_client.dart)
      │  ├─ data/
      │  │  ├─ models/ (question.dart, answer.dart, user_answer.dart)
      │  │  └─ services/ (quiz_api.dart, signalr_service.dart)
      │  ├─ features/quiz/
      │  │  ├─ quiz_controller.dart
      │  │  ├─ quiz_providers.dart
      │  │  ├─ quiz_screen.dart
      │  │  ├─ result_screen.dart
      │  │  └─ widgets/ (answer_chip.dart, question_card.dart, progress_bar.dart)
      │  ├─ app_theme.dart
      │  └─ main.dart
      ├─ pubspec.yaml
      └─ android/app/src/main/AndroidManifest.xml
</pre>

<h2 id="kurulum">⚙️ Kurulum</h2>

<h3>Backend</h3>
<ol>
  <li>.NET 8 SDK + SQL Server kurulu olmalı.</li>
  <li><code>backend/BKU/appsettings.json</code>:
    <pre><code>{
  "ServerURL": "http://0.0.0.0:5058",
  "ConnectionStrings": {
    "DefaultConnection": "Server=CSNBAHMETESER\\SQLEXPRESS01;Database=BKUDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  },
  "AllowedHosts": "*"
}</code></pre>
  </li>
  <li>Migration &amp; DB:
    <pre><code>cd backend/BKU
dotnet restore
dotnet tool install --global dotnet-ef   # ilk kez
dotnet ef database update                 # mevcut migration'ları uygula
# veya:
# dotnet ef migrations add Init
# dotnet ef database update</code></pre>
  </li>
  <li>Çalıştır:
    <pre><code>dotnet run</code></pre>
    <ul>
      <li>REST API: <code>http://localhost:5058/api/*</code></li>
      <li>SignalR Hub: <code>http://localhost:5058/hubs/quiz</code></li>
      <li>Loglar: <code>backend/BKU/logs/log-*.txt</code></li>
    </ul>
  </li>
</ol>

<h3>Flutter</h3>
<ol>
  <li>Flutter SDK 3.x kurulu olmalı.</li>
  <li><code>app/bku_v1/lib/core/config.dart</code>:
    <pre><code>// Emülatör
const baseUrl = 'http://10.0.2.2:5058';
// Fiziksel cihaz (aynı ağdaki PC IP'niz):
// const baseUrl = 'http://192.168.1.34:5058';

const apiBase = '$baseUrl/api';
const hubUrl  = '$baseUrl/hubs/quiz';
String? get jwtToken => null; // JWT kapalı</code></pre>
  </li>
  <li>Paketler &amp; Çalıştırma:
    <pre><code>cd app/bku_v1
flutter pub get
flutter run</code></pre>
  </li>
</ol>

<h2 id="api">🧩 REST API &amp; SignalR</h2>

<table>
  <thead>
    <tr><th>Endpoint</th><th>Metod</th><th>Açıklama</th></tr>
  </thead>
  <tbody>
    <tr><td>/api/Question</td><td>GET</td><td>Tüm sorular (+Answers)</td></tr>
    <tr><td>/api/Question</td><td>POST</td><td>Yeni soru ekle (body: <code>{ text, answers[] }</code>)</td></tr>
    <tr><td>/api/Question/{id}</td><td>GET/PUT/DELETE</td><td>CRUD</td></tr>
    <tr><td>/api/Answer</td><td>GET/POST</td><td>Şık CRUD</td></tr>
    <tr><td>/api/UserAnswers/submit</td><td>POST</td><td>Cevap gönder: <code>{ questionId, answerId }</code> → <code>{ correct: true/false }</code></td></tr>
  </tbody>
</table>

<p><b>SignalR</b></p>
<ul>
  <li>Hub: <code>/hubs/quiz</code></li>
  <li>Örnek olay: Sunucu yeni soru eklediğinde <code>QuestionAdded</code> olayı yayınlanır.</li>
</ul>

<h2 id="log">📝 Loglama</h2>
<ul>
  <li><b>Serilog</b> ile konsol + dosya (<code>backend/BKU/logs/log-*.txt</code>)</li>
  <li>EF Core &amp; SignalR için seviyeler optimize edilmiştir.</li>
</ul>

<h2 id="seed">🌱 Örnek SQL (soru & şık ekleme)</h2>
<pre><code>DECLARE @QuestionId INT;

INSERT INTO [dbo].[Questions] ([Text],[CreatedAt])
VALUES (N'Entity Framework Core nedir?', SYSUTCDATETIME());
SET @QuestionId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Answers] ([Text],[IsCorrect],[QuestionId]) VALUES
 (N'.NET için bir ORM kütüphanesidir.', 1, @QuestionId),
 (N'Front-end JavaScript framework''üdür.', 0, @QuestionId),
 (N'C şıkkı', 0, @QuestionId),
 (N'D şıkkı', 0, @QuestionId);</code></pre>

<h2 id="trouble">🛠️ Sorun Giderme</h2>
<ul>
  <li><b>Flutter emülatörü → Backend’e bağlanmıyor:</b> <code>10.0.2.2</code> kullanın. Gerçek cihazda PC’nin LAN IP’sini yazın.</li>
  <li><b>SignalR negotiate 500:</b> <code>MapHub</code>, CORS ve URL (HTTP/HTTPS) tutarlılığını kontrol edin.</li>
  <li><b>EF “Invalid column”:</b> Model &amp; migration uyumsuz. Gerekirse geçiş ekleyip <code>dotnet ef database update</code> çalıştırın.</li>
</ul>

<hr>
<p align="center">Made with ❤️ using ASP.NET Core & Flutter</p>
