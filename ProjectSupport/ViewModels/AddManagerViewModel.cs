﻿using ProjectSupport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.ViewModels
{
    public class AddManagerViewModel
    {
        public Project Project { get; set; }
        public bool HasManager { get; set; }
    }
}