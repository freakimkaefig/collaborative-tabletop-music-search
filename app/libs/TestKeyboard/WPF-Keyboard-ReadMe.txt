Read Me für WPFKeyboard

Die entscheidenden Dateien sind (mit deren C#-Dateien):
	- Caret.xaml
	- CustomKeyboard.xaml
	- KeyboardController.xaml


Als Veranschaulichung wurde KeyboardSample.xaml hinzugefügt.
Um eine funktionierende Tastatur zu erzeugen benötigt es lediglich einer Instanz von KeyboardController. 
Diese kann entweder per XAML (siehe KeyboardSample.xaml) oder natürlich per c# instanziiert werden.


Unabdingbar ist lediglich die Methode setFocusedElement(UIElement focusedElement). "focusedElement" muss hierbei min. vom Typ TextBox sein.
Allerdings benötigt es ein SurfaceTextbox, um den Caret auch per Touch-Events verschieben zu können. Dabei muss das Caret aktualisiert werden, was ebenfalls über die setFocusedElement(UIElement focusedElement) erfolgt.
Um unerwünschtes Verhalten zu verhindern sollte das SurfaceTextBox unbedingt auf nicht fokusierbar gesetzt werden (Focusable="False"). 
Als Beispiel dient KeyboardSample, was mit Kommentaren versehen wurde, wodurch einiges erklärt werden sollte.


Das Caret selbst funktioniert am besten, wenn das SurfaceTextBox-Element innerhalb eines Grids ist (Canvas müsste auch gehen). 
StackPanel oder ähnliche Panels, die das überaneinanderlegen von Elementen erschwert, kann zu unerwünschten Verhalten führen. Das wird ergänzt, sobald ich Zeit dafür habe.


Das Design ist nicht final und sollte ergänzt werden. Die optische Darstellung sollte in CustomKeyboard.xaml geändert werden.
Es wurde lediglich ein Style verwendet, der abgerundete Ecken und den Inhalt des Buttons zentriert. Zusätzlich wird die Farbe bei Tastendruck verändert.
Es wurden Kommentare hinzugefügt, was das Verständnis erleichtern soll.


Komplett optional gibt es auch ein Interface, an dem man sich registrieren kann, um die Tasteneingabe genausten mitverfolgen zu können.
Dies funktioniert mittels der Methode setCustomKeyboardListener(CustomKeyboardViewListener listener). Das Interface sieht wie folgt aus:

public interface CustomKeyboardViewListener
    {
        void typedKey(String key);
        void typedBackSpace();
        void typedArrow(int arrowIndex);
    }

Auch hierfür sind Kommentare bei KeyboardSample zu finden.

Ich hoffe, das reicht um sich in der Testanwendung zu Recht zu finden.
Bei Schwierigkeiten kann ich gerne kontaktiert werden. Es sind wahrscheinlich noch einige Bugs enthalten, weshalb ich es begrüßen würde, wenn ihr mir Bescheid gebt, sobald euch etwas auffällt, was innerhalb der bereits oben 
angesprochenen Beschränkungen ist.(Also z.B. dass es bei StackPanel nicht funktioniert weiß ich bereits ;) )

Schönen Tag noch,
Benedikt