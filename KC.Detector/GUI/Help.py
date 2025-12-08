import tkinter as tk
from tkinter import scrolledtext


class HelpDialog:
    window: tk.Toplevel
    text_widget: scrolledtext.ScrolledText
    button_frame: tk.Frame

    def __init__(self, parent: tk.Tk) -> None:
        self.window = tk.Toplevel(parent)
        self.window.title("Help")
        self.window.geometry("400x500")
        self.window.minsize(300, 500)
        self.window.resizable(True, True)
        self.setup_ui()

    def setup_ui(self) -> None:
        title_label = tk.Label(self.window, text="Help", font=("Segoe UI", 18, "bold"))
        title_label.pack(pady=(20, 10), padx=20)

        self.text_widget = scrolledtext.ScrolledText(
            self.window,
            wrap=tk.WORD,
            font=("Segoe UI", 10),
            padx=20,
            pady=20
        )
        self.text_widget.pack(fill=tk.BOTH, expand=True, padx=20, pady=10)

        self.text_widget.tag_config("heading", font=("Segoe UI", 12, "bold"))
        self.text_widget.tag_config("subheading", font=("Segoe UI", 10, "bold"))

        self.text_widget.insert(
            tk.END,
            "This program will help you learn card counting by letting you see the current "
            "count information, and will give you the steps for each hand recommended by "
            "the basic strategy.\n\n"
        )

        self.text_widget.insert(tk.END, "How does card counting work?", "heading")

        self.text_widget.insert(
            tk.END,
            "\nCard counting in essence is easy, you just have to keep a number in your mind "
            "that you will consistently increase or decrease based on what cards you see being dealt. "
            "This is called your running count\n\n"
        )

        self.text_widget.insert(
            tk.END,
            "If you see a card in the range of 2 to 6, it's a low card, you add one to your "
            "running count. If you see a card that's a 10, a ten-value card (J, Q, K), or an ace, "
            "you subtract one from your running count. If you see a 7 through 9, you don't have to "
            "add or subtract anything.\n\n"
        )

        self.text_widget.insert(
            tk.END,
            "The running count is not enough to determine what kind of cards are more likely. "
            "You have to keep in mind how many cards remain in the shoe. For this, take a look at "
            "how many cards have been played, and try to estimate the number of decks that were "
            "played (a deck is 52 cards).\n\n"
        )

        self.text_widget.insert(
            tk.END,
            "If you are playing at a 6-deck table, and see that two decks have been played, "
            "then you know you have 4 decks remaining. This is where your true count comes in: "
            "it's your running count divided by the estimated number of decks still in the shoe.\n\n"
        )

        self.text_widget.insert(tk.END, "What to do with my count?", "heading")

        self.text_widget.insert(
            tk.END,
            "\nIf your true count is high, that means that high ranking cards are more likely to come, "
            "if it is low, that means you can expect low ranking cards. You can deviate from the basic "
            "strategy based on this information and gain an advantage this way. If the count is high, "
            "it is recommended that you bet higher than usual.\n\n"
        )

        self.text_widget.insert(
            tk.END,
            "In the UI, you will see the current counts, so you can always check if you did your calculations correctly.\n\n"
        )

        self.text_widget.insert(tk.END,"How to use this program?", "heading")
        self.text_widget.insert(
            tk.END,
            "\nAll you have to do is:\n"
            "1. Start a game of blackjack on your computer, preferably in windowed mode.\n"
            "2. Start this program, and select the ROIs for the dealer's area, and for the players' area.\n"
            "3. Select the card dimensions for the dealers cards and the players' cards.\n"
            "4. Start the overlay, and position it as convenient.\n"
            "5. Start playing, and follow the recommendations shown on the overlay.\n"
            "For the ROIs and card sizes, you can write these to a file and load them later to avoid "
            "having to reselect them each time.\n\n"
        )

        self.text_widget.insert(tk.END, "Gamble responsibly.", "subheading")

        # Disable editing
        self.text_widget.config(state=tk.DISABLED)

        self.button_frame = tk.Frame(self.window)
        self.button_frame.pack(side=tk.BOTTOM, fill=tk.X, padx=20, pady=15)

        got_it_button = tk.Button(
            self.button_frame,
            text="Got it",
            command=self.window.destroy
        )
        got_it_button.pack(side=tk.RIGHT)

    def show(self) -> None:
        """Show the help dialog."""
        self.window.deiconify()
        self.window.grab_set()
        self.window.wait_window()
        return