using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    public interface IAutomapperMapping
    {
        TKTo Map<TFrom, TKTo>(TFrom from);
        TKTo Map<TFrom, TKTo>(TFrom from, TKTo to);
    }
}
