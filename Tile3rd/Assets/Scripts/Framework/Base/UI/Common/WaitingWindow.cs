using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace CSFramework
{
    public class WaitingWindow : WindowUI
    {
        private readonly Vector3 _rotate_direction = new Vector3(0, 0, 1);
        private WaitingWindowView _view;
        private SchedulerFloat _delay_scheduler;
        // private SchedulerFloat _auto_close_scheduler;
        private float _origin_alpha;

        private int _reference;

        protected override void on_create()
        {
            Property.UseCommonAnimation = false;
            Property.PlayOpenCloseSound = false;

            _view = GetComponent<WaitingWindowView>();
            _origin_alpha = _view.Background.color.a;

            _reference = 1;

            refresh_order();
        }

        public void Init (float delay_time = 0/*, float auto_close_time = 0*/)
        {
            if (delay_time > 0)
            {
                _delay_scheduler = new SchedulerFloat();
                _delay_scheduler.Init(delay_time);
                _view.WaitingRT.gameObject.SetActive(false);
                _view.Background.color = new Color(_view.Background.color.r, _view.Background.color.g, _view.Background.color.b, 0);
            }

            // if (auto_close_time > 0)
            // {
            //     _auto_close_scheduler = new SchedulerFloat();
            //     _auto_close_scheduler.Init(auto_close_time);
            // }
        }

        public void AddReference ()
        {
            ++_reference;
        }

        public void SubReference ()
        {
            --_reference;
            if (_reference == 0)
                Close();
        }

        private void Update ()
        {
            if (State == WindowState.Opened)
            {
                var dt = Time.deltaTime;
                _view.WaitingRT.Rotate(_rotate_direction, -dt * 200);

                if (_delay_scheduler != null && _delay_scheduler.Tick(dt))
                {
                    _view.WaitingRT.gameObject.SetActive(true);
                    _view.Background.color = new Color(_view.Background.color.r, _view.Background.color.g, _view.Background.color.b, _origin_alpha);
                    _delay_scheduler = null;
                }

                // if (_auto_close_scheduler != null && _auto_close_scheduler.Tick(dt))
                // {
                //     _auto_close_scheduler = null;
                //     Close();
                // }
            }
        }
    }
}