from rx import operators as ops
from rx.scheduler.mainloop import TkinterScheduler

from GUI.GuiConductor import GuiConductor
from ProcessConductor import ProcessConductor
from rx.scheduler.mainloop import TkinterScheduler
from rx import operators as ops


def main():
    # Create process conductor first
    process_conductor = ProcessConductor()
    
    # Create GUI conductor (this will initialize PyQt6 for overlay)
    gui = GuiConductor()

    # Set up observables
    gui.rois_selected_observable.subscribe(lambda rois: process_conductor.rois_selected_handler(rois))
    gui.start_detection_observable.subscribe(lambda _: process_conductor.start_detection())
    gui.stop_detection_observable.subscribe(lambda _: process_conductor.stop_detection())
    gui.card_sizes_selected_observable.subscribe(lambda sizes: process_conductor.card_sizes_selected_handler(sizes))
    gui.read_rois_and_card_dimensions_json_observable.subscribe(lambda filepath: process_conductor.read_rois_and_card_dimensions(filepath))
    gui.write_rois_and_card_dimensions_json_observable.subscribe(lambda filepath: process_conductor.write_rois_and_card_dimensions(filepath))
    process_conductor.done_reading_rois_and_card_dimensions_json_observable.subscribe(lambda json_data: gui.rois_selected(json_data.rois_container))
    process_conductor.done_reading_rois_and_card_dimensions_json_observable.subscribe(lambda json_data: gui.card_sizes_selected(json_data.sizes_container))

    main_thread_scheduler = TkinterScheduler(gui.overlay.window) #TODO: replace with PyQt scheduler if needed
    process_conductor.overlay_data_update_observable.pipe(ops.observe_on(main_thread_scheduler)).subscribe(lambda data: gui.update_overlay_model(data))

    # Load configuration
    process_conductor.read_basic_strategy("Assets/BasicStrategy.json")
    process_conductor.read_possible_messages("Assets/Messages.json")

    gui.start_main_window()

    return

if __name__ == "__main__":
    main()