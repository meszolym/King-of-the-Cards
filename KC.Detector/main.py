#Milyen elemekre van szükség:
# - minimal starter GUI (roi selector indítása, mentése, betöltése, lapszámolás indítása)
# - roi selector (kijelölés, mentés, betöltés)
# - lapok megtalálása (keretek keresése a roi-n belül)
# - lapértékek értelmezése (sarkok alapján maszkolással)
# - kiment lapok kezelése
# - lapok számlálása (értékek összeadása)
# - eredmény megjelenítése (összeg, lapok, hibák)

# Pipeline:
# 1. GUI indítása
# 2. ROI kijelölés (ha nincs elmentve)
# FŐ VONAL:
# 3. Képek megszerzése (ImageCapturer)
# 4. Képek vágása (ImageSplitter)
# 5. Képek feldolgozása (CardFinder, CardInterpreter + MessageFinder, MessageInterpreter)
# 6. Eredmény megjelenítése (GUI frissítése)

def main():
    return
if __name__ == "__main__":
    main()