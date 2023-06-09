﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VXEngine.Objects;

namespace VXEngine.Controllers;

/// <summary>
/// Controller for input
/// </summary>
public class BasicInputController {

    /// <summary>
    /// Mouse data structure that holds mouse's position and its movement delta
    /// </summary>
    public struct MouseData {
        public int X { get; set; }
        public int Y { get; set; }
        public int DiffX;
        public int DiffY;

        public MouseData( ) {
            X = 0;
            Y = 0;
            DiffX = 0;
            DiffY = 0;
        }
    }

    /// <summary>
    /// Config reference
    /// </summary>
    protected BasicConfigController _config { get; private set; }

    // Mouse states
    protected MouseState _currentMouse;
    protected MouseState _previousMouse;

    // Keyboard states
    protected KeyboardState _currentKeyboard;
    protected KeyboardState _previousKeyboard;

    /// <summary>
    /// Wheter events (like mouse clicks) should be propagated
    /// </summary>
    public bool PropagateEvent { get; private set; } = true;

    /// <summary>
    /// Absolute mouse data, relative to the window
    /// </summary>
    public MouseData MouseAbsolute { get; private set; }

    /// <summary>
    /// In-Game mouse data, transformed by e.g. camera
    /// </summary>
    public MouseData MouseInGame { get; private set; }

    /// <summary>
    /// For how long was the LMB (Left Mouse Button) pressed. In milliseconds
    /// </summary>
    public float LMBPressDuration { get; private set; }

    /// <summary>
    /// For how long was the MMB (Middle Mouse Button) pressed. In milliseconds
    /// </summary>
    public float MMBPressDuration { get; private set; }

    /// <summary>
    /// For how long was the RMB (Right Mouse Button) pressed. In milliseconds
    /// </summary>
    public float RMBPressDuration { get; private set; }

    /// <summary>
    /// For how long was each key on keyboard pressed. In milliseconds
    /// </summary>
    public Dictionary<Keys, float> KeyPressDuration { get; private set; }

    /// <summary>
    /// Function to calculate in-game mouse. In input you get <see cref="MouseAbsolute"/> and you can transform it e.g. by camera transformations, then return new X and Y of <see cref="MouseInGame"/>
    /// </summary>
    public Func<MouseData, Vector2> MouseInGameCalculation { get; private set; } = mouse => Vector2.Zero;

    /// <summary>
    /// Constructor
    /// </summary>
    public BasicInputController(BasicConfigController config) {
        _config = config;

        KeyPressDuration = new Dictionary<Keys, float>( );
        MouseAbsolute = new MouseData( );
        MouseInGame = new MouseData( );
        MouseInGameCalculation = mouse => new Vector2(
            (mouse.X - _config.ViewOffsetX) / _config.ViewScaleX,
            (mouse.Y - _config.ViewOffsetY) / _config.ViewScaleY
        );
    }

    /// <summary>
    /// Update input's logic
    /// </summary>
    public virtual void Update(GameTime time) {
        // Get previous tick references
        _previousMouse = _currentMouse;
        _previousKeyboard = _currentKeyboard;

        // Get current tick references
        _currentMouse = Mouse.GetState( );
        _currentKeyboard = Keyboard.GetState( );

        // Reset propagation
        PropagateEvent = true;

        // Set absolute data
        MouseData absoluteData = MouseAbsolute;
        absoluteData.DiffX = _currentMouse.X - _previousMouse.X;
        absoluteData.DiffY = _currentMouse.Y - _previousMouse.Y;
        absoluteData.X = _currentMouse.X;
        absoluteData.Y = _currentMouse.Y;

        // Calculate in-game positions
        MouseData inGameData = MouseInGame;
        int inGameX = inGameData.X;
        int inGameY = inGameData.Y;
        Vector2 newInGame = MouseInGameCalculation == null
            ? new Vector2(absoluteData.X, absoluteData.Y)
            : MouseInGameCalculation.Invoke(absoluteData);

        inGameData.X = (int)newInGame.X;
        inGameData.Y = (int)newInGame.Y;
        inGameData.DiffX = inGameData.X - inGameX;
        inGameData.DiffY = inGameData.Y - inGameY;

        // Set data
        MouseAbsolute = absoluteData;
        MouseInGame = inGameData;

        // Update mouse press duration
        LMBPressDuration = _currentMouse.LeftButton == ButtonState.Pressed ? LMBPressDuration + (float)time.ElapsedGameTime.TotalMilliseconds : 0;
        MMBPressDuration = _currentMouse.MiddleButton == ButtonState.Pressed ? MMBPressDuration + (float)time.ElapsedGameTime.TotalMilliseconds : 0;
        RMBPressDuration = _currentMouse.RightButton == ButtonState.Pressed ? RMBPressDuration + (float)time.ElapsedGameTime.TotalMilliseconds : 0;

        // Update key press duration
        if (!IsAnyKeyPressed( )) {
            KeyPressDuration.Clear( );
        } else {
            // Remove keys that are not pressed
            foreach (KeyValuePair<Keys, float> entry in KeyPressDuration)
                if (!_currentKeyboard.IsKeyDown(entry.Key))
                    KeyPressDuration.Remove(entry.Key);

            // Update time
            foreach (Keys key in _currentKeyboard.GetPressedKeys( ))
                // If key already is in the dictionary - update its time
                if (KeyPressDuration.ContainsKey(key))
                    KeyPressDuration[key] += (float)time.ElapsedGameTime.TotalMilliseconds;

                // Otherwise add that key
                else
                    KeyPressDuration.Add(key, (float)time.ElapsedGameTime.TotalMilliseconds);
        }
    }

    /// <summary>
    /// Checks if any key is pressed
    /// </summary>
    public virtual bool IsAnyKeyPressed( ) => _currentKeyboard.GetPressedKeyCount( ) > 0;

    /// <summary>
    /// Checks if given key is pressed
    /// </summary>
    public virtual bool IsKeyPressed(Keys key) => _currentKeyboard.IsKeyDown(key);

    /// <summary>
    /// Checks if given key is pressed for given time in milliseconds
    /// </summary>
    public virtual bool IsKeyPressedFor(Keys key, float durationInMs) => _currentKeyboard.IsKeyDown(key) && KeyPressDuration.ContainsKey(key) && KeyPressDuration[key] >= durationInMs;

    /// <summary>
    /// Checks if given key was just pressed
    /// </summary>
    public virtual bool IsKeyPressedOnce(Keys key) => _currentKeyboard.IsKeyDown(key) && !_previousKeyboard.IsKeyDown(key);

    /// <summary>
    /// Checks if given key was just released
    /// </summary>
    public virtual bool IsKeyReleased(Keys key) => !_currentKeyboard.IsKeyDown(key) && _previousKeyboard.IsKeyDown(key);

    /// <summary>
    /// Checks if all given keys are pressed
    /// </summary>
    public virtual bool AreAllKeysPressed(Keys[] keys) => keys.All(key => _currentKeyboard.IsKeyDown(key));

    /// <summary>
    /// Checks if all given keys are pressed for given time in milliseconds
    /// </summary>
    public virtual bool AreAllKeysPressedFor(Keys[] keys, float durationInMs) => keys.All(key => _currentKeyboard.IsKeyDown(key) && KeyPressDuration.ContainsKey(key) && KeyPressDuration[key] >= durationInMs);

    /// <summary>
    /// Checks if any of given keys is pressed
    /// </summary>
    public virtual bool AreAnyKeysPressed(Keys[] keys) => keys.Any(key => _currentKeyboard.IsKeyDown(key));

    /// <summary>
    /// Checks if any of given keys is pressed for given time in milliseconds
    /// </summary>
    public virtual bool AreAnyKeysPressedFor(Keys[] keys, float durationInMs) => keys.Any(key => _currentKeyboard.IsKeyDown(key) && KeyPressDuration.ContainsKey(key) && KeyPressDuration[key] >= durationInMs);

    /// <summary>
    /// Checks if LMB (Left Mouse Button) is pressed
    /// </summary>
    public virtual bool IsLMBPressed( ) => PropagateEvent && _currentMouse.LeftButton == ButtonState.Pressed;

    /// <summary>
    /// Checks if LMB (Left Mouse Button) is pressed for a given duration in milliseconds
    /// </summary>
    public virtual bool IsLMBPressedFor(float durationInMs) => PropagateEvent && _currentMouse.LeftButton == ButtonState.Pressed && LMBPressDuration >= durationInMs;

    /// <summary>
    /// Checks if LMB (Left Mouse Button) was just pressed
    /// </summary>
    public virtual bool IsLMBPressedOnce( ) => PropagateEvent && _currentMouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton != ButtonState.Pressed;

    /// <summary>
    /// Checks if LMB (Left Mouse Button) was just released
    /// </summary>
    public virtual bool IsLMBReleased( ) => PropagateEvent && _currentMouse.LeftButton != ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Pressed;

    /// <summary>
    /// Checks if MMB (Middle Mouse Button) is pressed
    /// </summary>
    public virtual bool IsMMBPressed( ) => PropagateEvent && _currentMouse.MiddleButton == ButtonState.Pressed;

    /// <summary>
    /// Checks if MMB (Middle Mouse Button) is pressed for a given duration in milliseconds
    /// </summary>
    public virtual bool IsMMBPressedFor(float durationInMs) => PropagateEvent && _currentMouse.LeftButton == ButtonState.Pressed && MMBPressDuration >= durationInMs;

    /// <summary>
    /// Checks if MMB (Middle Mouse Button) was just pressed
    /// </summary>
    public virtual bool IsMMBPressedOnce( ) => PropagateEvent && _currentMouse.MiddleButton == ButtonState.Pressed && _previousMouse.MiddleButton != ButtonState.Pressed;

    /// <summary>
    /// Checks if MMB (Middle Mouse Button) was just released
    /// </summary>
    public virtual bool IsMMBReleased( ) => PropagateEvent && _currentMouse.MiddleButton != ButtonState.Pressed && _previousMouse.MiddleButton == ButtonState.Pressed;

    /// <summary>
    /// Checks if RMB (Right Mouse Button) is pressed
    /// </summary>
    public virtual bool IsRMBPressed( ) => PropagateEvent && _currentMouse.RightButton == ButtonState.Pressed;

    /// <summary>
    /// Checks if RMB (Right Mouse Button) is pressed for a given duration in milliseconds
    /// </summary>
    public virtual bool IsRMBPressedFor(float durationInMs) => PropagateEvent && _currentMouse.LeftButton == ButtonState.Pressed && RMBPressDuration >= durationInMs;

    /// <summary>
    /// Checks if RMB (Right Mouse Button) was just pressed
    /// </summary>
    public virtual bool IsRMBPressedOnce( ) => PropagateEvent && _currentMouse.RightButton == ButtonState.Pressed && _previousMouse.RightButton != ButtonState.Pressed;

    /// <summary>
    /// Checks if RMB (Right Mouse Button) was just released
    /// </summary>
    public virtual bool IsRMBReleased( ) => PropagateEvent && _currentMouse.RightButton != ButtonState.Pressed && _previousMouse.RightButton == ButtonState.Pressed;

    /// <summary>
    /// Checks if LMB (Left Mouse Button) or RMB (Right Mouse Button) is pressed
    /// </summary>
    public virtual bool IsMousePressed( ) => IsLMBPressed( ) || IsRMBPressed( );

    /// <summary>
    /// Checks if LMB (Left Mouse Button) or RMB (Right Mouse Button) was just pressed
    /// </summary>
    /// <returns></returns>
    public virtual bool IsMousePressedOnce( ) => (IsLMBPressedOnce( ) && !IsRMBPressedOnce( )) || (IsRMBPressedOnce( ) && !IsLMBPressedOnce( ));

    /// <summary>
    /// Checks if LMB (Left Mouse Button) or RMB (Right Mouse Button) was just released
    /// </summary>
    /// <returns></returns>
    public virtual bool IsMouseReleased( ) => (IsLMBReleased( ) && !IsRMBReleased( )) || (IsRMBReleased( ) && !IsLMBReleased( ));

    /// <summary>
    /// Checks if mouse was just scrolled up
    /// </summary>
    /// <returns></returns>
    public virtual bool HasScrolledUp( ) => _currentMouse.ScrollWheelValue > _previousMouse.ScrollWheelValue;

    /// <summary>
    /// Checks if mouse was just scrolled down
    /// </summary>
    /// <returns></returns>
    public virtual bool HasScrolledDown( ) => _currentMouse.ScrollWheelValue < _previousMouse.ScrollWheelValue;

    /// <summary>
    /// Checks if mouse was just scrolled in any direction
    /// </summary>
    /// <returns></returns>
    public virtual bool HasScrolled( ) => _currentMouse.ScrollWheelValue != _previousMouse.ScrollWheelValue;

    /// <summary>
    /// Stops event propagation. If function is triggered, it will block any next events of mouse for given tick (logic's loop pass)
    /// </summary>
    public virtual void StopPropagation( ) => PropagateEvent = false;

    /// <summary>
    /// Checks if <see cref="MouseAbsolute"/> is over given position
    /// </summary>
    public virtual bool IsOver(float x, float y, float width, float height) => PropagateEvent && MouseAbsolute.X >= x && MouseAbsolute.X <= x + width && MouseAbsolute.Y >= y && MouseAbsolute.Y <= y + height;

    /// <summary>
    /// Checks if <see cref="MouseInGame"/> is over given position
    /// </summary>
    public virtual bool IsOverInGame(float x, float y, float width, float height) => PropagateEvent && MouseInGame.X >= x && MouseInGame.X <= x + width && MouseInGame.Y >= y && MouseInGame.Y <= y + height;

    /// <summary>
    /// Checks if <see cref="MouseInGame"/> is over given object
    /// </summary>
    public virtual bool IsOver(GameObject obj) => obj == null
        ? false
        : PropagateEvent && MouseInGame.X >= obj.RenderX && MouseInGame.X <= obj.RenderX + obj.RenderWidth && MouseInGame.Y >= obj.RenderY && MouseInGame.Y <= obj.RenderY + obj.RenderHeight;

}