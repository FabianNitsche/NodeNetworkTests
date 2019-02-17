﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using ReactiveUI;
using UcrPoc.ViewModels.Editors;
using UcrPoc.ViewModels.Ports;
using UcrPoc.Views.Nodes;

namespace UcrPoc.ViewModels.Nodes
{
    public class DynamicButtonToAxisNode : NodeViewModel
    {
        private readonly List<ValueNodeInputViewModel<bool?>> _inputs = new List<ValueNodeInputViewModel<bool?>>();
        private readonly Subject<short?> _output = new Subject<short?>();

        private readonly BehaviorSubject<bool?> _addInputButtonState = new BehaviorSubject<bool?>(false);

        public bool? AddInputButtonState
        {
            get => _addInputButtonState.Value;
            set => _addInputButtonState.OnNext(value);
        }

        public short? DefaultSetPointValue { get; set; } = 0;

        static DynamicButtonToAxisNode()
        {
            //Splat.Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<DynamicButtonToAxisNode>));
            Splat.Locator.CurrentMutable.Register(() => new DynamicButtonToAxisView(), typeof(IViewFor<DynamicButtonToAxisNode>));
        }

        public DynamicButtonToAxisNode()
        {
            Name = "Dynamic Button\nTo Axis";

            _addInputButtonState.Subscribe(OnAddInput);

            var output = new ValueNodeOutputViewModel<short?>
            {
                Name = "Output",
                Port = new AxisPortViewModel(),
                Value = _output,
            };
            Outputs.Add(output);
        }

        private void OnAddInput(bool? state)
        {
            if (state == null || (bool) !state) return;
            AddInput();
        }

        public void AddInput()
        {
            var inputNum = _inputs.Count;
            var vm = new ValueNodeInputViewModel<bool?>
            {
                Name = $"Input {inputNum + 1}",
                Port = new ButtonPortViewModel(),
                Editor = new ButtonToAxisEditorViewModel(),
                HideEditorIfConnected = false
            };
            _inputs.Add(vm);
            Inputs.Add(vm);
            vm.ValueChanged.Subscribe(newValue =>
            {
                if (Inputs.Count < inputNum || newValue == null) return;
                var value = (bool) newValue;
                if (value)
                {
                    var sp = ((ButtonToAxisEditorViewModel)(Inputs[inputNum].Editor)).AxisSetPoint;
                    _output.OnNext(sp);
                }
                else if (DefaultSetPointValue != null)
                {
                    _output.OnNext(DefaultSetPointValue);
                }
                //Console.WriteLine($@"Input {inputNum + 1} changed to: {newValue} - Setpoint: {sp}");
            });
        }
    }
}
