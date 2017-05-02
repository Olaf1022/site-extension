﻿using LetsEncrypt.Azure.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsEncrypt.Azure.Core.Services
{
    public interface ICertificateService
    {
        void Install(ICertificateInstallModel model);

        List<string> RemoveExpired(int removeXNumberOfDaysBeforeExpiration = 0);
    }
}
