using System;
using System.Collections.Generic;
using AfyaHMIS.Models.Concepts;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AfyaHMIS.Service
{
    public interface IConceptService
    {
        public List<SelectListItem> GetConceptAnswersIEnumerable(Concept concept);
    }

    public class ConceptService : IConceptService
    {
        private readonly ICoreService ICoreService = new CoreService();

        public ConceptService()
        {

        }

        public List<SelectListItem> GetConceptAnswersIEnumerable(Concept concept)
        {
            return ICoreService.GetIEnumerable("SELECT ca_answer, ct_name FROM ConceptAnswer INNER JOIN Concept ON ca_answer=ct_idnt WHERE ca_concept=" + concept.Id);
        }
    }
}
