# Skillproof – Kompetencia-alapú toborzási platform

A Skillproof egy modern, professzionális hálózatépítő és álláskereső webalkalmazás, amely a munkáltatókat és a munkavállalókat köti össze. A platform fókuszában a tudásalapú kiválasztás áll: a cégek az álláshirdetésekhez saját kompetenciateszteket rendelhetnek, amelyeket a jelölteknek a jelentkezés során ki kell tölteniük.

## Célkitűzések

- Objektív, valós tudáson alapuló szűrés biztosítása a munkáltatók számára.
- A jelentkezési és kiválasztási folyamat hatékonyságának növelése.
- Átlátható felület biztosítása a pályázóknak a képességeik bizonyítására és az ideális pozíciók megtalálására.

## Technológiai Stack és Fejlesztési Környezet

- **Backend:** C# .NET 9
- **Frontend:** Angular 21
- **Adatbázis:** MSSQL

### Adatbázis indítása és futtatás

A backend elindításához és az adatbázis migrációk futtatásához nyiss egy terminált a szerver oldali gyökérmappában:

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update
dotnet run
```

A frontend indítása a kliens mappában:

```bash
npm install
ng serve
```

## Szerepkörök és Felhasználók

- **Munkáltató (B2B):** Álláshirdetések és az azokhoz tartozó tesztek létrehozása, jelentkezők kezelése és eredményeik kiértékelése.
- **Pályázó (B2C):** Profil menedzselése, állások böngészése, mentése, tesztek kitöltése és jelentkezés a kiválasztott pozíciókra.
- **Rendszer adminisztrátor (Admin):** A platform globális felügyelete.

## Csapat és Felelősségek

- **[@BenjaminKovacs09](https://github.com/BenjaminKovacs09)** – Project Manager
- **[@Marci260](https://github.com/Marci260)** – Architect
- **[@zadoriaron](https://github.com/zadoriaron)** – Fullstack fejlesztő
- **[@KelemenOzseb](https://github.com/KelemenOzseb)** – Fullstack fejlesztő
- **[@oli-tolnai](https://github.com/oli-tolnai)** – Fullstack fejlesztő
- **[@AdamRevesz](https://github.com/AdamRevesz)** – Fullstack fejlesztő

## Tervezett funkciók

### 1. Publikus felületek

- Értékajánlat kommunikálása (gyorsabb, tesztalapú kiválasztás).
- Regisztráció és bejelentkezés különválasztott munkáltatói és pályázói folyamattal.

### 2. Munkáltatói funkciók

- **Munkáltatói Dashboard:** Az aktív és lezárt hirdetések kártyás áttekintése.
- **Álláshirdetések kezelése:**
  - Új pozíciók létrehozása (cím, leírás, elvárások).
  - Meglévő hirdetések módosítása vagy törlése.
- **Teszt- és kérdésbank:**
  - Kérdések és komplett kompetenciatesztek összeállítása.
  - Tesztek hozzárendelése konkrét álláshirdetésekhez.
- **Jelentkezések adminisztrációja:**
  - Pályázók listázása egy adott pozícióra.
  - A kitöltött teszteredmények megtekintése a jelentkezők profilja mellett.
  - AI asszisztált kiértékelés: A kifejtős kérdésekre adott válaszokat a rendszer mesterséges intelligencia segítségével előzetesen kiértékeli, amelyet a munkáltató felülvizsgálhat és szükség esetén manuálisan felülírhat.

### 3. Pályázói funkciók

- **Profilkezelés:**
  - Szakmai tapasztalatok, készségek és személyes adatok szerkesztése.
  - Skillek hozzárendelése a profilhoz, vannak olyan skillek amelyeket teszt kitöltésével érhet el a felhasználó
- **Álláskeresés:**
  - Elérhető pozíciók listázása és részleteik megtekintése.
  - Állások kedvencekhez adása (könyvjelzőzés későbbi megtekintésre).
- **Jelentkezési folyamat:**
  - Pályázás indítása a felületen keresztül.
  - Kompetenciateszt kitöltése: Amennyiben a munkáltató tesztet rendelt az álláshoz, annak integrált kitöltése a jelentkezési folyamat részeként.
  - Sikeres jelentkezés véglegesítése és visszajelzés a felhasználónak.
