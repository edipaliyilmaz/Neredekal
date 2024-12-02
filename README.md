Proje Başlatma Dokümantasyonu
Bu proje, Docker kullanarak PostgreSQL, PgAdmin, RabbitMQ, Elasticsearch ve Kibana servislerini çalıştırmanızı sağlar. Aşağıdaki adımları takip ederek projenizi başlatabilirsiniz.

Gerekli Araçlar
Docker: Projeyi çalıştırmak için Docker kurulu olmalıdır. Docker İndir
Docker Compose: Birden fazla konteyneri kolayca yönetmek için Docker Compose gereklidir. Docker Compose İndir
Proje Yapılandırması
Bu projede aşağıdaki servisler Docker üzerinde çalıştırılacaktır:

PostgreSQL: Veritabanı yönetim sistemi
PgAdmin: PostgreSQL veritabanını yönetmek için web tabanlı bir araç
RabbitMQ: Mesaj kuyruklama sistemi
Elasticsearch: Veri arama ve analiz aracı
Kibana: Elasticsearch ile entegre çalışan görselleştirme ve analiz aracı
Adım 1: Docker ve Docker Compose Kurulumu
Öncelikle Docker ve Docker Compose'un sisteminize kurulu olduğundan emin olun. Eğer kurulu değilse, aşağıdaki bağlantılardan kurulum yönergelerini takip edebilirsiniz:

Docker Kurulum Rehberi
Docker Compose Kurulum Rehberi
Adım 2: Docker Compose Dosyasını Yapılandırma
Projeye ait docker-compose.yml dosyasını oluşturun veya mevcut bir docker-compose.yml dosyasını aşağıdaki şekilde yapılandırın:

yaml
Copy code
version: '3'
services:
  postgres:
    image: postgres:latest
    environment:
      POSTGRES_USER: youruser
      POSTGRES_PASSWORD: yourpassword
      POSTGRES_DB: yourdatabase
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    networks:
      - backend_network

  pgadmin:
    image: dpage/pgadmin4
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@admin.com
      PGADMIN_DEFAULT_PASSWORD: admin
    ports:
      - "80:80"
    networks:
      - backend_network

  rabbitmq:
    image: rabbitmq:management
    ports:
      - "5672:5672"
      - "15672:15672"
    networks:
      - backend_network

  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:8.4.0
    environment:
      - discovery.type=single-node
    ports:
      - "9200:9200"
    networks:
      - backend_network

  kibana:
    image: docker.elastic.co/kibana/kibana:8.4.0
    environment:
      - ELASTICSEARCH_HOSTS=http://elasticsearch:9200
    ports:
      - "5601:5601"
    networks:
      - backend_network

networks:
  backend_network:
    driver: bridge

volumes:
  postgres_data:
Açıklamalar:

PostgreSQL: youruser, yourpassword, yourdatabase kısmını kendi kullanıcı adı, şifreniz ve veritabanı adıyla değiştirin.
PgAdmin: PgAdmin için varsayılan kullanıcı adı ve şifreyi değiştirebilirsiniz.
RabbitMQ: RabbitMQ yönetim paneline http://localhost:15672 adresinden erişebilirsiniz.
Elasticsearch ve Kibana: Kibana'ya http://localhost:5601 adresinden erişebilirsiniz.
Adım 3: Docker Compose ile Servisleri Başlatma
docker-compose.yml dosyasının bulunduğu dizinde terminali açarak aşağıdaki komutu çalıştırın:

bash
Copy code
docker-compose up -d
Bu komut, tüm gerekli konteynerleri arka planda çalıştıracaktır. Konteynerlerin doğru bir şekilde çalıştığını kontrol etmek için aşağıdaki komutu kullanabilirsiniz:

bash
Copy code
docker-compose ps
Adım 4: Servislere Erişim
PostgreSQL: PostgreSQL veritabanınıza localhost:5432 üzerinden erişebilirsiniz. Kullanıcı adı ve şifreyi docker-compose.yml dosyasındaki ayarlarla eşleştirin.
PgAdmin: http://localhost adresinden PgAdmin web arayüzüne erişebilirsiniz. Giriş için e-posta adresi ve şifreyi docker-compose.yml dosyasındaki ayarlardan alın.
RabbitMQ: http://localhost:15672 üzerinden RabbitMQ yönetim paneline erişebilirsiniz. Varsayılan kullanıcı adı guest, şifre guesttir.
Elasticsearch: http://localhost:9200 üzerinden Elasticsearch'a HTTP ile erişebilirsiniz.
Kibana: http://localhost:5601 üzerinden Kibana arayüzüne erişebilirsiniz.
Adım 5: Uygulama Başlatma
Projeyi başlatmak için gerekli olan her şeyi Docker konteynerlerinde çalıştırdıktan sonra uygulamanızı başlatabilirsiniz. Eğer ek yapılandırmalar veya bağımlılıklar varsa, bunları README'ye eklemeyi unutmayın.

Adım 6: Docker Compose ile Servisleri Durdurma
Projeyi durdurmak için aşağıdaki komutu kullanabilirsiniz:

bash
Copy code
docker-compose down
Bu komut, konteynerleri durdurur ancak veritabanı ve diğer veri dosyaları kaybolmaz.

Alternatif Docker Compose Yapılandırması
Aşağıda, Elasticsearch ve Kibana için alternatif bir yapılandırma bulunmaktadır:

yaml
Copy code
version: '3.8'
services:
  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:8.7.1
    expose:
      - 9200
    environment:
      - xpack.security.enabled=false
      - "discovery.type=single-node"
      - ELASTIC_USERNAME=elastic
      - ELASTIC_PASSWORD=changeme
    networks:
      - es-net
    ports:
      - 9200:9200
    volumes:
      - elasticsearch-data:/usr/share/elasticsearch/data
  kibana:
    image: docker.elastic.co/kibana/kibana:8.7.1
    environment:
      - ELASTICSEARCH_HOSTS=http://elasticsearch:9200
    expose:
      - 5601
    networks:
      - es-net
    depends_on:
      - elasticsearch
    ports:
      - 5601:5601
    volumes:
      - kibana-data:/usr/share/kibana/data

networks:
  es-net:
    driver: bridge

volumes:
  elasticsearch-data:
    driver: local
  kibana-data:
    driver: local
Bu adımlar, PostgreSQL, PgAdmin, RabbitMQ, Elasticsearch ve Kibana'nın Docker üzerinde çalıştırılmasını sağlar ve uygulamanın başlangıç yapması için gerekli ortamı hazırlar.