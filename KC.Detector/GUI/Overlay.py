from rx.scheduler.mainloop import TkinterScheduler

from CardCounting.HandValueLogic import hand_value_from_hand

import tkinter as tk
from typing import Optional

from Models.BoundingBox import BoundingBox
from Models.OverlayModel import OverlayModel

def move_to_string(move: int) -> str:
    if move is None:
        return "..."

    move_map: dict[int, str] = {
        1: "Hit",
        2: "Stand",
        3: "Double/Hit",
        4: "Double/Stand",
        5: "Split",
        0: "..."
    }
    return move_map.get(move, "...")

class Overlay:
    window: tk.Tk
    placement: BoundingBox
    current_overlay: Optional[OverlayModel]
    dealer_label: tk.Label
    hands_label: tk.Label
    running_count_label: tk.Label
    true_count_label: tk.Label
    scheduler: TkinterScheduler

    def __init__(self, placement) -> None:
        self.placement = placement
        self.current_overlay = None


        self.window = tk.Tk()
        self.window.title("Blackjack Display")

        self.scheduler = TkinterScheduler(self.window)

        x = int(placement.x)
        y = int(placement.y)
        width = int(placement.w)
        height = int(placement.h)

        self.window.geometry(f"{width}x{height}+{x}+{y}")
        self.window.attributes('-topmost', True)
        self.window.resizable(True, True)

        self.setup_ui()

    def setup_ui(self) -> None:
        self.dealer_label = tk.Label(self.window, text="Dealer: --", anchor="w", font=('Courier', 10))
        self.dealer_label.pack(fill='x', padx=5, pady=(5, 2))

        self.running_count_label = tk.Label(self.window, text="Running count: --", anchor="w", font=('Courier', 10))
        self.running_count_label.pack(fill='x', padx=5, pady=2)

        self.true_count_label = tk.Label(self.window, text="True count: --", anchor="w", font=('Courier', 10))
        self.true_count_label.pack(fill='x', padx=5, pady=2)

        self.hands_label = tk.Label(self.window, text="", anchor="nw", justify='left', font=('Courier', 10))
        self.hands_label.pack(fill='both', expand=True, padx=5, pady=(5, 5))

    def update_from_overlay_model(self, overlay_model) -> None:
        self.current_overlay = overlay_model

        dealer_value = str(hand_value_from_hand(overlay_model.dealer_hand_info.hand))
        self.dealer_label.config(text=f"Dealer: {dealer_value}")

        running_count = overlay_model.table_running_count
        true_count = overlay_model.table_true_count

        self.running_count_label.config(text=f"Running count: {running_count}")
        self.true_count_label.config(text=f"True count: {true_count}")

        hands_text: str = self.build_hands_text()
        self.hands_label.config(text=hands_text)

    def build_hands_text(self) -> str:
        if not self.current_overlay or not self.current_overlay.player_hand_info:
            return ""

        lines = []
        for idx, hand_record in enumerate(self.current_overlay.player_hand_info):
            hand_value: str = str(hand_value_from_hand(hand_record.hand))
            recommendation: str = move_to_string(hand_record.recommended_move)
            lines.append(f"Hand {idx + 1}: {hand_value} ({recommendation})")

        return "\n".join(lines)

    def run(self) -> None:
        self.window.mainloop()

    def show_overlay(self) -> None:
        self.window.deiconify()

    def hide_overlay(self):
        self.window.withdraw()
