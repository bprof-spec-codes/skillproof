# skillproof
##  Adatbázis beállítása helyi fejlesztéshez (Docker)

Mivel a projekt Linux/Mac környezetben is futtatható, az adatbázist (MSSQL) egy Docker konténerben futtatjuk. Nincs szükség bonyolult lokális telepítésekre!

### Előfeltételek
* **Docker** és **Docker Compose** telepítve legyen a gépeden.
* **.NET EF Core Tools** telepítve legyen globálisan vagy lokálisan (`dotnet tool install dotnet-ef`).

### 1. Adatbázis szerver indítása
Nyiss egy terminált a projekt gyökerében (ahol a `docker-compose.yml` van), és futtasd ezt:
```bash
docker compose up -d
