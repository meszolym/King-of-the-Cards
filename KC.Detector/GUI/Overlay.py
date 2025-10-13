from tkinter import *

from screeninfo import get_monitors

from Models import Enums
from Models.OverlayModel import OverlayModel


class Overlay:
    CONST_Y_OFFSET = 10 # Offset y to avoid overlap with cards

    window : Tk
    data : OverlayModel

    def __init__(self, data: OverlayModel):
        self.data = data
        return

    def _setup_window(self):

        self.window = Tk()
        self.window.overrideredirect(True)
        # self.window.attributes("-topmost", True)
        # self.window.attributes("-transparentcolor", "black")
        # monitor = get_monitors()[0]
        # self.window.geometry(f"{monitor.width}x{monitor.height}")
        self.update_overlay()


    def update_overlay(self):

        for widget in self.window.winfo_children():
            if isinstance(widget, Label):
                widget.destroy()

        # Add labels for player hands
        for x, y, score, move in self.data.player_hand_info:
            move_info = Enums.Move(move).name if move != Enums.Move.Unknown else "No advice"
            text = f"{score}\n{move_info}"
            text += f"\n{x},{y}"  # For debugging position
            player_label = Label(self.window, text=text, font=("Arial", 16), bg="black", fg="yellow")
            # player_label.place(x=x, y=y + self.CONST_Y_OFFSET)
            player_label.pack(pady=5)

        # Add label for dealer hand
        dealer_x, dealer_y, dealer_score = self.data.dealer_hand_info
        dealer_label = Label(self.window, text=f"{dealer_score}", font=("Arial", 16), bg="black", fg="yellow")
        # dealer_label.place(x=dealer_x, y=dealer_y + self.CONST_Y_OFFSET)
        dealer_label.pack(pady=5)

        # Add label for counts
        count_text = f"Running Count: {self.data.table_running_count}\nTrue Count: {self.data.table_true_count}"
        count_label = Label(self.window, text=count_text, font=("Arial", 16), bg="black", fg="yellow")
        # count_label.place(x=10, y=10)
        count_label.pack(pady=5)

        return

    def show_overlay(self):
        self._setup_window()
        self.window.mainloop()
        return


    def hide_overlay(self):
        self.window.destroy()
        return

