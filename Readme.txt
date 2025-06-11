Docker-compose.yml dosyasında elasticserach ve kibana imajları var.
Terminalden bunları ayağıya kaldırmak gerekiyor.
Elasticsearch için: docker-compose up -d elasticsearch kibana
Kibana için: docker-compose up -d kibana
ikisini birden aynı anda çalıştırmak için: docker-compose up -d elasticsearch kibana

Uygulama ayağıya kalktığında swager açılıyor orada endpointlere ve body'e bakıp postman üzerinden istek atabilirsin.

End pointlere istek attıktan sonra kibanaya loglar düşer. 

Kibana’yı aç
Tarayıcıda:

http://localhost:5601
Data View (Index Pattern) Oluştur

Sol alttan Management (⚙️) → Stack Management → Data Views’e tıkla.

Sağ üstte Create data view butonuna bas.

Index pattern olarak girin:

authserver-logs-*
Time field olarak @timestamp seç.

“Create data view” ile kaydet.

Discover’da Log’ları İncele

Sol menüden Discover’a geç.

Üstteki data view dropdown’undan authserver-logs-* seçeneğini seç.

Sağ üstteki zaman filtresini “Last 15 minutes” veya “Today” yap.

Alt kısımda 82 kayıt listelenecek; hangi endpoint’lerin, hangi mesajların (e.g. “Starting registration for…”, “Login attempt…”) geldiğini filtreleyerek görebilirsin.

Örnek Filtreleme

Arama satırına yaz:

vbnet
Kopyala
message:"Starting registration for"
Veya sol tarafta Add filter ile Level is Information gibi kısıtlamalar ekle.

Bunlarla uçtan uca:

API → register/login

Serilog → Elasticsearch’e log

Kibana → Discover’da gerçek zamanlı log takibi

akışını tamamlamış olacaksın.