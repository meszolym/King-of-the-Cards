from dataclasses import dataclass
from tkinter import *
from screeninfo import get_monitors
from Models import Enums

@dataclass
class OverlayModel:
    player_hand_info: list[tuple[int, int, str, Enums.Move]] # x, y, score, recommended move
    dealer_hand_score: str
    table_running_count: int
    table_true_count: int

class Overlay:
    window : Tk
    data : OverlayModel

    def __init__(self, data: OverlayModel):
        self.data = data
        return

    def _setup_window(self):
        self.window = Tk()
        self.window.overrideredirect(True)
        self.window.attributes("-topmost", True)
        self.window.attributes("-transparentcolor", "black")
        monitor = get_monitors()[0]
        self.window.geometry(f"{monitor.width}x{monitor.height}")

        # Add labels for player hands
        for x, y, score, move in self.data.player_hand_info:
            move_info = Enums.Move(move).name if move != Enums.Move.Unknown else "No advice"
            text = f"{score}\n{move_info}"
            player_label = Label(self.window, text=text, font=("Arial", 16), bg="black", fg="white")
            player_label.place(x=x, y=y)

    def show_overlay(self):
        self._setup_window()
        self.window.mainloop()
        return


    def hide_overlay(self):
        self.window.destroy()
        return

    def update_overlay(self):
        return