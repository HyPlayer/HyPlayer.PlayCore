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
        public Chopin(IPlayCoreWrapper wrapper)
        {
            _wrapper = wrapper;
        } 
    }
}
