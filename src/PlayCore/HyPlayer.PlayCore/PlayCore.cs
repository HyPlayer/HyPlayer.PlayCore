using System;
using System.Collections.Generic;
using System.Text;
using HyPlayer.PlayCore.Abstraction.Interfaces;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;
using HyPlayer.PlayCore.Wrapper;
using HyPlayer.PlayCore.Abstraction.Interfaces.Wrapper;

namespace HyPlayer.PlayCore
{
    public partial class Chopin : PlayCoreBase
    {
        public override AudioServiceBase? CurrentAudioService
            { get => _wrapper.CurrentAudioService; protected set { if (value == null) _wrapper.SetCurrentAudioService(typeof(object)); else _wrapper.SetCurrentAudioService(value.GetType()); } }
        public override PlayControllerBase? CurrentPlayListController
            { get => _wrapper.CurrentPlayController; protected set { if (value == null) _wrapper.SetCurrentPlayistController(typeof(object)); else _wrapper.SetCurrentPlayistController(value.GetType()); } }

        public Chopin(IPlayCoreWrapper wrapper)
        {
            _wrapper = wrapper;
        } 
    }
}
