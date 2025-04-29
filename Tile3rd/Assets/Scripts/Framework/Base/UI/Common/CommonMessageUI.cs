using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace CSFramework
{
    public class CommonMessageUI : WindowUI
    {
        public class AnimationFrame
        {
            public Vector2 Position;
            public float Opacity;
            public float Duration;
            public AnimationFrame (Vector2 position, float opacity, float duration)
            {
                Position = position;
                Opacity = opacity;
                Duration = duration;
            }
        }
        protected static List<AnimationFrame> C_AnimationFrameList = new List<AnimationFrame>
        {
            new AnimationFrame(new Vector2(0, -60), 0, 0.36f), // 渐显
            new AnimationFrame(new Vector2(0, 0), 1, 0.72f), // 静止
            new AnimationFrame(new Vector2(0, 0), 1, 0.36f), // 渐隐
            new AnimationFrame(new Vector2(0, 60), 0, 0),
        };

        public class Message : BaseUI
        {
            private Image _image;
            private Text _text;
            private int _current_frame_index;
            private float _escape_time;
            protected override void on_create ()
            {
                _image = GetComponent<Image>();
                _text = find_component<Text>("Text");
            }

            public void Init (string msg)
            {
                gameObject.SetActive(true);
                _text.text = msg;
                _current_frame_index = 0;
                _escape_time = 0;
                apply_frame(C_AnimationFrameList[_current_frame_index]);
            }

            protected void apply_frame (AnimationFrame current_frame, AnimationFrame next_frame = null, float escape_time = 0.0f)
            {
                var position = current_frame.Position;
                var opacity = current_frame.Opacity;
                var duration = current_frame.Duration;

                if (next_frame != null)
                {
                    var percent = escape_time / duration;
                    position += (next_frame.Position - current_frame.Position) * percent;
                    opacity += (next_frame.Opacity - current_frame.Opacity) * percent;
                }

                transform.localPosition = position;
                
                _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, opacity);
                _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, opacity);
            }
            
            public bool Tick (float dt)
            {
                var end_tick = false;
                if (_current_frame_index < C_AnimationFrameList.Count - 1)
                {
                    var current_frame = C_AnimationFrameList[_current_frame_index];
                    var next_frame = C_AnimationFrameList[_current_frame_index + 1];

                    _escape_time += dt;
                    if (_escape_time < current_frame.Duration)
                    {
                        apply_frame(current_frame, next_frame, _escape_time);
                    }
                    else
                    {
                        _escape_time = 0;
                        ++_current_frame_index;
                        apply_frame(current_frame, next_frame, current_frame.Duration);
                    }
                }
                else if (_current_frame_index < C_AnimationFrameList.Count)
                {
                    apply_frame(C_AnimationFrameList[_current_frame_index]);
                    end_tick = true;
                }
                else
                {
                    end_tick = true;
                }
                return end_tick;
            }
        }

        private Transform _message_group;
        private GameObject _message_template;
        private List<Message> _message_list;
        protected override void on_create ()
        {
            _message_group = transform.Find("MessageGroup");
            _message_template = _message_group.Find("Message").gameObject;
            _message_template.SetActive(false);
            _message_list = new List<Message>();
        }

        public void AddMessage (string msg)
        {
            var message = create_ui<Message>(_message_template, _message_group);
            message.Init(msg);
            _message_list.Add(message);
        }

        // protected override void on_tick (float dt)
        private void Update() 
        {
            var dt = Time.deltaTime;
            for (var i = _message_list.Count - 1; i >= 0; --i)
            {
                var message = _message_list[i];
                if (message.Tick(dt))
                {
                    destroy_ui(message);
                    _message_list.RemoveAt(i);
                }
            }
        }
    }
}