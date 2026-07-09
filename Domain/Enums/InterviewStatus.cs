namespace Domain.Enums;

public enum InterviewStatus
{
    All, // для фильтров
    Scheduled, // Запланированный
    WaitingForVerdict, // Ожидает решения
    Canceled, // Отклонен
    Completed // Завершен
}