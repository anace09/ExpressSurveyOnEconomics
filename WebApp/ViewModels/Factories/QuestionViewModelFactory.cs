using WebApp.Helpers;
using WebApp.Models;
using WebApp.ViewModels;

public static class QuestionViewModelFactory
{
    public static QuestionViewModel Build(TaskQuestion q)
    {
        QuestionViewModel vm = q switch
        {
            TrueFalseQuestion tf => new TrueFalseQuestionViewModel
            {
                Text = LocalizationHelper.Pick(tf.TextKk, tf.TextRu),
            },
            MultipleChoiceQuestion mc => ((Func<MultipleChoiceQuestionViewModel>)(() =>
            {
                var shuffled = LocalizationHelper.Pick(mc.OptionsKk, mc.OptionsRu)
                    .Select((opt, idx) => (opt, idx))
                    .OrderBy(_ => Guid.NewGuid())
                    .ToList();
                return new MultipleChoiceQuestionViewModel
                {
                    Options = shuffled.Select(x => x.opt).ToList(),
                    OptionIndices = shuffled.Select(x => x.idx).ToList(),
                };
            }))(),
            CategorizationQuestion cat => ((Func<CategorizationQuestionViewModel>)(() =>
            {
                var shuffled = LocalizationHelper.Pick(cat.TermsKk, cat.TermsRu)
                    .Select((term, idx) => (term, idx))
                    .OrderBy(_ => Guid.NewGuid())
                    .ToList();
                return new CategorizationQuestionViewModel
                {
                    Category1 = LocalizationHelper.Pick(cat.Category1Kk, cat.Category1Ru),
                    Category2 = LocalizationHelper.Pick(cat.Category2Kk, cat.Category2Ru),
                    Terms = shuffled.Select(x => x.term).ToList(),
                    TermIndices = shuffled.Select(x => x.idx).ToList(),
                };
            }))(),
            MatchingQuestion mq => ((Func<MatchingQuestionViewModel>)(() =>
            {
                var shuffled = LocalizationHelper.Pick(mq.RightItemsKk, mq.RightItemsRu)
                    .Select((item, idx) => (item, idx))
                    .OrderBy(_ => Guid.NewGuid())
                    .ToList();
                return new MatchingQuestionViewModel
                {
                    LeftItems = LocalizationHelper.Pick(mq.LeftItemsKk, mq.LeftItemsRu),
                    RightItems = shuffled.Select(x => x.item).ToList(),
                    RightItemIndices = shuffled.Select(x => x.idx).ToList(),
                };
            }))(),
            TableQuestion tq => new TableQuestionViewModel
            {
                Headers = LocalizationHelper.Pick(tq.HeadersKk, tq.HeadersRu),
                TableData = LocalizationHelper.Pick(tq.TableDataKk, tq.TableDataRu),
                EditableColumns = tq.EditableColumns,
            },
            _ => throw new NotSupportedException()
        };

        vm.Id = q.Id;
        vm.Order = q.Order;
        vm.Text = LocalizationHelper.Pick(q.TextKk, q.TextRu);
        vm.Points = q.Points;
        vm.Type = q switch
        {
            TrueFalseQuestion => QuestionType.TrueFalse,
            MultipleChoiceQuestion => QuestionType.MultipleChoice,
            CategorizationQuestion => QuestionType.Categorization,
            MatchingQuestion => QuestionType.Matching,
            TableQuestion => QuestionType.Table,
            _ => QuestionType.Unknown
        };

        return vm;
    }
}
