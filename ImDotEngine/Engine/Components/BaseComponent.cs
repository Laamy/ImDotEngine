using SFML.Graphics;
using SFML.Window;

class BaseComponent
{
    public virtual void OnUpdate(RenderWindow ctx) { }
    public virtual void OnFixedUpdate() { }

    public virtual void Initialized() { }

    public virtual void Closing() { }

    public virtual void Focus() { }
    public virtual void LostFocus() { }

    public virtual void Resize(SizeEventArgs e) { }

    public virtual void JoystickButtonPressed(JoystickButtonEventArgs e) { }
    public virtual void JoystickButtonReleased(JoystickButtonEventArgs e) { }
    public virtual void JoystickConnected(JoystickConnectEventArgs e) { }
    public virtual void JoystickDisconnected(JoystickConnectEventArgs e) { }
    public virtual void JoystickMoved(JoystickMoveEventArgs e) { }

    public virtual void KeyPressed(KeyEventArgs e) { }
    public virtual void KeyReleased(KeyEventArgs e) { }

    public virtual void MouseButtonPressed(MouseButtonEventArgs e) { }
    public virtual void MouseButtonReleased(MouseButtonEventArgs e) { }
    public virtual void MouseMoved(MouseMoveEventArgs e) { }
    public virtual void MouseWheelScrolled(MouseWheelScrollEventArgs e) { }
    public virtual void MouseEntered() { }
    public virtual void MouseLeft() { }

    public virtual void SensorChanged(SensorEventArgs e) { }
    public virtual void TextEntered(TextEventArgs e) { }

    public virtual void TouchBegan(TouchEventArgs e) { }
    public virtual void TouchEnded(TouchEventArgs e) { }
    public virtual void TouchMoved(TouchEventArgs e) { }
}