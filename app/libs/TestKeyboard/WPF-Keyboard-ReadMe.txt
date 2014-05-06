Read Me f�r WPFKeyboard

Die entscheidenden Dateien sind (mit deren C#-Dateien):
	- Caret.xaml
	- CustomKeyboard.xaml
	- KeyboardController.xaml


Als Veranschaulichung wurde KeyboardSample.xaml hinzugef�gt.
Um eine funktionierende Tastatur zu erzeugen ben�tigt es lediglich einer Instanz von KeyboardController. 
Diese kann entweder per XAML (siehe KeyboardSample.xaml) oder nat�rlich per c# instanziiert werden.


Unabdingbar ist lediglich die Methode setFocusedElement(UIElement focusedElement). "focusedElement" muss hierbei min. vom Typ TextBox sein.
Allerdings ben�tigt es ein SurfaceTextbox, um den Caret auch per Touch-Events verschieben zu k�nnen. Dabei muss das Caret aktualisiert werden, was ebenfalls �ber die setFocusedElement(UIElement focusedElement) erfolgt.
Um unerw�nschtes Verhalten zu verhindern sollte das SurfaceTextBox unbedingt auf nicht fokusierbar gesetzt werden (Focusable="False"). 
Als Beispiel dient KeyboardSample, was mit Kommentaren versehen wurde, wodurch einiges erkl�rt werden sollte.


Das Caret selbst funktioniert am besten, wenn das SurfaceTextBox-Element innerhalb eines Grids ist (Canvas m�sste auch gehen). 
StackPanel oder �hnliche Panels, die das �beraneinanderlegen von Elementen erschwert, kann zu unerw�nschten Verhalten f�hren. Das wird erg�nzt, sobald ich Zeit daf�r habe.


Das Design ist nicht final und sollte erg�nzt werden. Die optische Darstellung sollte in CustomKeyboard.xaml ge�ndert werden.
Es wurde lediglich ein Style verwendet, der abgerundete Ecken und den Inhalt des Buttons zentriert. Zus�tzlich wird die Farbe bei Tastendruck ver�ndert.
Es wurden Kommentare hinzugef�gt, was das Verst�ndnis erleichtern soll.


Komplett optional gibt es auch ein Interface, an dem man sich registrieren kann, um die Tasteneingabe genausten mitverfolgen zu k�nnen.
Dies funktioniert mittels der Methode setCustomKeyboardListener(CustomKeyboardViewListener listener). Das Interface sieht wie folgt aus:

public interface CustomKeyboardViewListener
    {
        void typedKey(String key);
        void typedBackSpace();
        void typedArrow(int arrowIndex);
    }

Auch hierf�r sind Kommentare bei KeyboardSample zu finden.

Ich hoffe, das reicht um sich in der Testanwendung zu Recht zu finden.
Bei Schwierigkeiten kann ich gerne kontaktiert werden. Es sind wahrscheinlich noch einige Bugs enthalten, weshalb ich es begr��en w�rde, wenn ihr mir Bescheid gebt, sobald euch etwas auff�llt, was innerhalb der bereits oben 
angesprochenen Beschr�nkungen ist.(Also z.B. dass es bei StackPanel nicht funktioniert wei� ich bereits ;) )

Sch�nen Tag noch,
Benedikt