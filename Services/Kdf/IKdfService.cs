﻿namespace ASP_32.Services.Kdf
{
    public interface IKdfService
    {
        String Dk(String password, String salt);
    }
}
