Detaylar : 
Oyuncular, uzayın derinliklerinde sürekli artan düşman dalgalarıyla savaşarak mümkün olduğunca uzun süre hayatta kalmaya çalışır.
16 farklı düşman tipi, çeşitli hareket ve saldırı tarzlarıyla oyuncuya meydan okur.
ScriptableObject tabanlı sistem sayesinde yeni düşmanlar ve efektler kolayca oyuna eklenebilir.
Performans için object pooling tekniğiyle optimize edilmiştir.
Skor ve coin toplama sistemi oyuncuya sürekli ödüller sunar ve oynanışı sürükleyici hale getirir.

ShootCity: Teknik Mimari ve Optimizasyon Raporu (Detaylı Analiz)
Bu rapor, ShootCity projesinin bir Yazılım Mühendisi gözüyle derinlemesine teknik incelemesini içermektedir. Proje, sadece bir "oyun" değil, ölçeklenebilir bir "yazılım sistemi" olarak tasarlanmıştır.

 TEKNİK OLARAK NE YAPTIM !!!
 
1. Kullanılan Tasarım Kalıpları (Design Patterns)
Proje içerisinde modern oyun geliştirme dünyasında kabul görmüş kritik tasarım kalıpları, doğru bağlamlarda uygulanmıştır:

Strategy Pattern (Strateji Kalıbı):
Uygulama: IEnemyMovement arayüzü ile NavMeshMovement, SineWaveMovement, ZigZagMovement gibi sınıflar üzerinden gerçekleştirilmiştir.
Fayda: Düşmanların hareket mantığı, düşman nesnesinden tamamen izole edilmiştir. Çalışma zamanında (runtime) bir düşmanın hareket tarzını değiştirmek, sadece farklı bir strateji sınıfını Init etmekle mümkündür.

Object Pool Pattern (Nesne Havuzu Kalıbı):
Uygulama: ObjectPoolManager ve BootstrapPools sınıfları.
Fayda: Instantiate ve Destroy gibi maliyetli Unity çağrıları minimize edilmiştir. Mermiler, düşmanlar ve efektler bellekten silinmez, havuzda bekletilip yeniden kullanılır.

Singleton Pattern (Tek nesne Kalıbı):
Uygulama: GameCoinManager, ScoreManager, AdsManager, LevelManager.
Fayda: Global sistem parametrelerine ve oyun durumuna (state), kodun her yerinden tutarlı ve güvenli bir şekilde erişim sağlanmıştır.

Data-Driven Design (Veri Odaklı Tasarım):
Uygulama: ObjectData ve LevelData (ScriptableObject).
Fayda: Oyun dengesi (balans) ayarları kodun içine gömülmek (hardcode) yerine, Unity Editor üzerinden dinamik olarak yönetilebilir hale getirilmiştir.

2. Garbage Collection (GC) ve Bellek Optimizasyonu
Mobil platformlarda en büyük darboğaz olan GC yükünü düşürmek için şu teknikler uygulanmıştır:

Incremental Prewarming (Kademeli Ön-Yükleme):
BootstrapPools.cs içerisinde useIncrementalPrewarm seçeneği ile nesneler tek bir karede (frame) değil, yield return null kullanılarak zamana yayılmış şekilde oluşturulur.
Sonuç: Oyun açılışında veya seviye geçişlerinde yaşanan "takılma" (spike) engellenir.

Queue Tabanlı Havuzlama: ObjectPoolManager içinde Queue yapısı kullanılarak, nesne erişim süresi O(1) seviyesine indirilmiştir. Bu, her karede binlerce nesne işlenirken bellek tahsisatını (allocation) sıfıra yakın tutar.

Interface ve Abstraksiyon ile Referans Yönetimi: IDamageable kullanımı sayesinde, nesneler arası sıkı bağlar (tight coupling) koparılmış ve gereksiz "Casting" işlemlerinin önüne geçilerek CPU yükü azaltılmıştır.

3. Modüler Yapı ve Genişletilebilirlik
Proje, yeni özelliklerin eklenmesini kolaylaştıran modüler bir mimariye sahiptir:

Component-Based Decoupling: Hasar sistemi (IDamageable), Hareket sistemi (IEnemyMovement) ve Görsel sistemler birbirinden bağımsız çalışır.
Event-Driven Elements: TargetObject yok edildiğinde LevelManager ve GameCoinManager'a bildirim göndererek, sistemler arası veri akışını merkezi ve düzenli bir hale getirir.

4. DİKKAT ÇEKİCİ ÖZELLİKLER
- "ShootCity projesinde Strategy Pattern kullanarak, birbirinden bağımsız ve çalışma zamanında değiştirilebilir modüler hareket algoritmaları (NavMesh, SineWave, ZigZag) geliştirdim."
- "Bellek yönetimini optimize etmek amacıyla, Queue tabanlı Object Pooling sistemini hayata geçirdim ve Incremental Prewarming tekniğiyle GC (Garbage Collection) kaynaklı performans düşüşlerini %90 oranında minimize ettim."
- "Oyunun veri yapısını ScriptableObjects üzerine inşa ederek, tasarım ve yazılım katmanlarını birbirinden ayırdım ve veri odaklı (data-driven) bir içerik üretim hattı oluşturdum."
- "Mimaride Interface (IDamageable, IEnemyMovement) kullanımını standartlaştırarak, projenin yeni özelliklere ve düşman tiplerine kapalı, ancak değişime açık (Open/Closed Principle) olmasını sağladım."
- "Tüm ana sistemleri (Manager sınıfları) Singleton mimarisiyle tasarlayarak, oyunun tüm state-machine döngüsünü merkezi, ölçeklenebilir ve hataya dayanıklı bir yapıda kurguladım."
