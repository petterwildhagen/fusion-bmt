using api.GQL;
using api.Services;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq;
using System.Collections.Generic;
using Xunit;

namespace tests
{
    [Collection("UsesDbContext")]
    public class MutationTest : DbContextTestSetup
    {
        /* Primary Services*/
        protected readonly Mutation _mutation;
        protected readonly ProjectService _projectService;
        protected readonly EvaluationService _evaluationService;
        protected readonly ParticipantService _participantService;
        protected readonly QuestionService _questionService;
        protected readonly AnswerService _answerService;
        protected readonly ActionService _actionService;
        protected readonly NoteService _noteService;
        protected readonly ClosingRemarkService _closingRemarkService;

        /* Admin Services */
        protected readonly QuestionTemplateService _questionTemplateService;

        /* Other Services */
        protected readonly MockAuthService _authService;

        /* Helpers */
        private readonly Project _project;

        public MutationTest()
        {
            ILoggerFactory factory = new NullLoggerFactory();
            _projectService = new ProjectService(_context);
            _evaluationService = new EvaluationService(_context);
            _participantService = new ParticipantService(_context);
            _questionService = new QuestionService(_context);
            _answerService = new AnswerService(_context);
            _actionService = new ActionService(_context);
            _noteService = new NoteService(_context);
            _closingRemarkService = new ClosingRemarkService(_context);
            _questionTemplateService = new QuestionTemplateService(_context);
            _authService = new MockAuthService();
            _mutation = new Mutation(
                _projectService,
                _evaluationService,
                _participantService,
                _questionService,
                _answerService,
                _actionService,
                _noteService,
                _closingRemarkService,
                _questionTemplateService,
                _authService,
                new Logger<Mutation>(factory)
            );

            _project = _projectService.Create("Project");
        }

        /* Mutation wrappers with default values */

        protected Evaluation CreateEvaluation(
            string name = null,
            string projectId = null,
            string previousEvaluationId = null)
        {
            if (name == null)
            {
                name = Randomize.String();
            }

            if (projectId == null)
            {
                projectId = _project.Id;
            }

            if (previousEvaluationId == null)
            {
                previousEvaluationId = "";
            }

            Evaluation evaluation =  _mutation.CreateEvaluation(
                name: name,
                projectId: projectId,
                previousEvaluationId:  previousEvaluationId
            );

            return evaluation;
        }

        protected Participant CreateParticipant(
            Evaluation evaluation,
            string azureUniqueId = null,
            Organization? organization = null,
            Role? role = null)
        {
            if (azureUniqueId == null)
            {
                azureUniqueId = Randomize.Integer().ToString();
            }

            Participant participant = _mutation.CreateParticipant(
                azureUniqueId: azureUniqueId,
                evaluationId: evaluation.Id,
                organization: organization.GetValueOrDefault(Randomize.Organization()),
                role: role.GetValueOrDefault(Randomize.Role())
            );

            return participant;
        }

        protected Answer SetAnswer(
            string questionId,
            Severity? severity = null,
            string text = null,
            Progression? progression = null)
        {

            if (text == null)
            {
                text = Randomize.String();
            }

            Answer answer= _mutation.SetAnswer(
                    questionId: questionId,
                    severity: severity.GetValueOrDefault(Randomize.Severity()),
                    text: text,
                    progression: progression.GetValueOrDefault(Randomize.Progression())
            );

            return answer;
        }

        protected Action CreateAction(
            string questionId,
            string assignedToId,
            string description = null,
            System.DateTimeOffset? dueDate = null,
            Priority? priority = null,
            string title = null)
        {

            if (title == null)
            {
                title = Randomize.String();
            }

            if (description == null)
            {
                description = Randomize.String();
            }

            Action action = _mutation.CreateAction(
                questionId: questionId,
                assignedToId: assignedToId,
                description: description,
                dueDate: dueDate.GetValueOrDefault(System.DateTimeOffset.Now),
                priority: priority.GetValueOrDefault(Randomize.Priority()),
                title: title
            );

            return action;
        }

        protected Action EditAction(
            string actionId,
            string assignedToId,
            string description = null,
            System.DateTimeOffset? dueDate = null,
            Priority? priority = null,
            string title = null,
            bool onHold = false,
            bool completed = false)
        {

            if (title == null)
            {
                title = Randomize.String();
            }

            if (description == null)
            {
                description = Randomize.String();
            }

            Action action = _mutation.EditAction(
                actionId: actionId,
                assignedToId: assignedToId,
                description: description,
                dueDate: dueDate.GetValueOrDefault(System.DateTimeOffset.Now),
                priority: priority.GetValueOrDefault(Randomize.Priority()),
                title: title,
                onHold: onHold,
                completed: completed
            );

            return action;
        }

        protected void DeleteAction(string actionId)
        {
            _mutation.DeleteAction(actionId);
        }

        protected Note CreateNote(
            string actionId,
            string text = null)
        {

            if (text == null)
            {
                text = Randomize.String();
            }

            Note note = _mutation.CreateNote(
                actionId: actionId,
                text: text
            );

            return note;
        }

        protected ClosingRemark CreateClosingRemark(
            string actionId,
            string text = null)
        {

            if (text == null)
            {
                text = Randomize.String();
            }

            ClosingRemark remark = _mutation.CreateClosingRemark(
                actionId: actionId,
                text: text
            );

            return remark;
        }

        /* Helper methods */

        protected int NumberOfParticipants(Evaluation evaluation)
        {
            int participants = _participantService
                .GetAll()
                .Where(p => p.Evaluation == evaluation)
                .Count()
            ;

            return participants;
        }

        protected int NumberOfAnswers(Question question)
        {
            int answers = _answerService
                .GetAll()
                .Where(a => a.Question == question)
                .Count()
            ;

            return answers;
        }

        protected int NumberOfActions(Question question)
        {
            int actions = _actionService
                .GetAll()
                .Where(a => a.Question == question)
                .Count()
            ;

            return actions;
        }

        protected int NumberOfNotes(Action action)
        {
            int notes = _noteService
                .GetAll()
                .Where(a => a.Action == action)
                .Count()
            ;

            return notes;
        }

        protected int NumberOfClosingRemarks(Action action)
        {
            int remarks = _closingRemarkService
                .GetAll()
                .Where(a => a.Action == action)
                .Count()
            ;

            return remarks;
        }

        protected Question GetFirstQuestion(Evaluation evaluation)
        {
            return _questionService
                .GetAll()
                .Where(q => q.Evaluation.Id == evaluation.Id)
                .First()
            ;
        }
    }
}
