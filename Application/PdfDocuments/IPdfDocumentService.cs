namespace Application.PdfDocuments;

public interface IPdfDocumentService
{
    byte[] CreateInterviewProtocol(InterviewProtocolModel interview);

    byte[] CreateCandidateCard(CandidateCardModel candidate);

    byte[] CreateApplicationLetter(ApplicationLetterModel letter, LetterType type);
}
