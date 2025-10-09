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

from GUI.GuiConductor import GuiConductor
from GUI.Overlay import OverlayModel
from ProcessConductor import ProcessConductor

def main():
    process_conductor = ProcessConductor()
    gui = GuiConductor()

    gui.rois_selected_observable.subscribe(lambda rois: process_conductor.rois_selected_handler(rois))
    gui.start_detection_observable.subscribe(lambda _: process_conductor.start_preprocessor())
    gui.stop_detection_observable.subscribe(lambda _: process_conductor.stop_preprocessor())
    gui.card_sizes_selected_observable.subscribe(lambda sizes: process_conductor.card_sizes_selected_handler(sizes))
    gui.read_rois_and_card_dimensions_json_observable.subscribe(lambda filepath: process_conductor.read_rois_and_card_dimensions(filepath))
    gui.write_rois_and_card_dimensions_json_observable.subscribe(lambda filepath: process_conductor.write_rois_and_card_dimensions(filepath))
    process_conductor.done_reading_rois_and_card_dimensions_json_observable.subscribe(lambda json_data: gui.rois_selected(json_data.rois_container))
    process_conductor.done_reading_rois_and_card_dimensions_json_observable.subscribe(lambda json_data: gui.card_sizes_selected(json_data.sizes_container))
    process_conductor.overlay_data_update_observable.subscribe(lambda data: gui.update_overlay_model(data))

    process_conductor.read_basic_strategy("Assets/BasicStrategy.json")
    process_conductor.read_possible_messages("Assets/Messages.json")

    gui.start_main_window()

    return

if __name__ == "__main__":
    main()