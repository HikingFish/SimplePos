using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Infrastructure;
public class ApplicationUser : IdentityUser<Guid>
{
    //Link to domain user
    public Guid DomainUserId { get; set; }
}
