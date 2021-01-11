import React from 'react'
import { Question, Progression } from '../api/models'
import { Typography, Divider, SideSheet } from '@equinor/eds-core-react'
import { barrierToString } from '../utils/EnumToString'
import SingleAnswerSummary from './SingleAnswerSummary'
import { progressionLessThan } from '../utils/ProgressionStatus'
import Sticky from './Sticky'

interface AnswerSummarySidebarProps
{
    open: boolean
    onCloseClick: () => void
    question: Question
    questionNumber: number
    viewProgression: Progression
}

const AnswerSummarySidebar = ({ open, onCloseClick, question, questionNumber, viewProgression }: AnswerSummarySidebarProps) => {
    const answers = question.answers.filter(answer => progressionLessThan(answer.progression, viewProgression))

    const IndividualAssessmentAnswers = answers.filter(a => a.progression === Progression.IndividualAssessment)
    const alignmentAnswers = answers.filter(a => a.progression === Progression.Alignment)
    const workshopAnswers = answers.filter(a => a.progression === Progression.Workshop)

    return <Sticky>
        <SideSheet
            title={ barrierToString(question.barrier) }
            open={open}
            variant='large'
            onClose={onCloseClick}
            style={{position: 'relative'}}
        >
            <Typography variant="h4">{questionNumber}. {question.text}</Typography>
            { answers.length === 0 &&
                <p>No submitted answers</p>
            }
            {workshopAnswers.length !== 0 && <>
                <Divider />
                <Typography variant="h5">Workshop</Typography>
            </>}
            {workshopAnswers.map((answer) => {
                return <SingleAnswerSummary answer={answer} key={answer.id} />
            })}
            {alignmentAnswers.length !== 0 && <>
                <Divider />
                <Typography variant="h5">Alignment</Typography>
            </>}
            {alignmentAnswers.map((answer) => {
                return <SingleAnswerSummary answer={answer} key={answer.id} />
            })}
            {IndividualAssessmentAnswers.length !== 0 && <>
                <Divider />
                <Typography variant="h5">IndividualAssessment</Typography>
            </>}
            {IndividualAssessmentAnswers.map((answer) => {
                return <SingleAnswerSummary answer={answer} key={answer.id} />
            })}
        </SideSheet>
    </Sticky>
}

export default AnswerSummarySidebar
