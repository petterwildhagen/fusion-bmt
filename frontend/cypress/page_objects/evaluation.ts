import { Progression } from '../../src/api/models'
import { progressionToString } from '../../src/utils/EnumToString'

export class EvaluationPage {
    progressionStepLink = (progression: Progression, state: 'In Progress' | 'Complete' | 'Awaiting' | '' = '') => {
        /**
         * We can't identify stepper itself, without stepper content.
         * We also have no ids to tie up to. Thus at the moment search
         * the whole document for links with expected progression text.
         * Seems to hold as of now.
         */
        return cy.contains('a', new RegExp(`${progressionToString(progression)}.*${state}`))
    }
}

export class QuestionnaireSidePanel {
    body = () => {
        return cy
            .get("div[role='tablist']")
            .children()
            .eq(0)
            .invoke('attr', 'aria-controls')
            .then(id => {
                return cy
                    .get('#' + id)
                    .children()
                    .eq(0)
                    .children()
                    .eq(0)
                    .children()
                    .eq(0)
            })
    }
}
