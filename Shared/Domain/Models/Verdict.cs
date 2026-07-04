using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models;

public class Verdict
{
    // { InterviewId, UserId } составной ключ
    public int InterviewId { get; set; }
    public int UserId { get; set; }
    public string Decision { get; set; }
    public string Comment { get; set; }

    // навигационные свойства
    public Interview Interview { get; set; }
    public User User { get; set; }
}
