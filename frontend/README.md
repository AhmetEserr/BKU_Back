# React Native Quiz App

Uygulama artık çok ekranlı bir yapıya sahip. İlk ekranda kullanıcı `api/auth/login` adresine giriş yapar. Başarılı girişin ardından yıllar listesine yönlendirilir ve buradan bir yıl seçerek soruları görebilir.

1. **Login Ekranı** – Kullanıcı adı ve şifre girer, `api/auth/login`a POST atılır.
2. **Yıl Listesi** – 2015–2020 arası yılları buton olarak gösterir.
3. **Soru Listesi** – Seçilen yıl için `/api/question/year/{year}` endpointinden soruları çeker ve cevaplarıyla listeler.

Kurulum için dizinde `npm install` komutunu çalıştırın. Ardından `npm start` ile Metro sunucusunu başlatıp uygulamayı React Native ortamınızda çalıştırabilirsiniz.
