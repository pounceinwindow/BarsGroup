using PdfDocuments.Contracts;

namespace PdfDocuments;

public interface IPdfDocumentService
{
    byte[] CreateInterviewProtocol(InterviewProtocolModel interview);
    byte[] CreateCandidateCard(CandidateCardModel candidate);
    byte[] CreateApplicationLetter(CandidateCardModel candidate, CandidateApplicationModel application, LetterType type);
}
