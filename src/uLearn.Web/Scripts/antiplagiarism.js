$(document).ready(function () {
    $('.antiplagiarism__data').each(function () {
        let $self = $(this);
        let originalSubmissionId = $self.data('originalSubmissionId');
        let plagiarismSubmissionId = $self.data('plagiarismSubmissionId');

        let $originalSubmission = $('.code[data-submission-id="' + originalSubmissionId + '"]');
        let $plagiarismSubmission = $('.code[data-submission-id="' + plagiarismSubmissionId + '"]');
        
        let antiplagiarismData = JSON.parse($self[0].innerHTML);       
        let plagiarismData = antiplagiarismData.plagiarism;

        let originalTokens = getTokensDictionaryByIndex(antiplagiarismData.tokens_positions);
        let plagiarismTokens = getTokensDictionaryByIndex(plagiarismData.tokens_positions);

        $.each(plagiarismData.matched_snippets, function (idx, matchedSnippet) {
            let originalSubmissionFirstTokenIndex = matchedSnippet.original_submission_first_token_index;
            let plagiarismSubmissionFirstTokenIndex = matchedSnippet.plagiarism_submission_first_token_index;
            let tokensCount = matchedSnippet.snippet_tokens_count;
            let snippetFrequency = matchedSnippet.snippet_frequency;
            
            let originalSnippetStartPosition = originalTokens[originalSubmissionFirstTokenIndex].start_position;
            let originalSnippetFinishPosition = originalTokens[originalSubmissionFirstTokenIndex + tokensCount - 1].finish_position;
            let plagiarismSnippetStartPosition = plagiarismTokens[plagiarismSubmissionFirstTokenIndex].start_position;
            let plagiarismSnippetFinishPosition = plagiarismTokens[plagiarismSubmissionFirstTokenIndex + tokensCount - 1].finish_position;
            
            let originalCodeMirror = $originalSubmission[0].codeMirrorEditor;
            originalSnippetStartPosition = originalCodeMirror.getDoc().posFromIndex(originalSnippetStartPosition);
            originalSnippetFinishPosition = originalCodeMirror.getDoc().posFromIndex(originalSnippetFinishPosition);
                        
            let plagiarismCodeMirror = $plagiarismSubmission[0].codeMirrorEditor;
            plagiarismSnippetStartPosition = plagiarismCodeMirror.getDoc().posFromIndex(plagiarismSnippetStartPosition);
            plagiarismSnippetFinishPosition = plagiarismCodeMirror.getDoc().posFromIndex(plagiarismSnippetFinishPosition);

            console.log(matchedSnippet, plagiarismSnippetStartPosition, plagiarismSnippetFinishPosition);
            
            const snippetColor = getRandomColor();

            originalCodeMirror.getDoc().markText(originalSnippetStartPosition, originalSnippetFinishPosition, {
                className: 'antiplagiarism__matched-snippet',
                css: 'background-color: ' + snippetColor,
            });            
            plagiarismCodeMirror.getDoc().markText(plagiarismSnippetStartPosition, plagiarismSnippetFinishPosition, {
                className: 'antiplagiarism__matched-snippet',
                css: 'background-color: ' + snippetColor,
            });
        });
    });

    function getRandomColor() {
        // 30 random hues with step of 12 degrees
        let hue = Math.floor(Math.random() * 30) * 12;

        return $.Color({
            hue: hue,
            saturation: 0.9,
            lightness: 0.9,
            alpha: 0.5
        }).toRgbaString();
    }
    
    function getTokensDictionaryByIndex(tokensPositionsArray) {
        let result = {};
        $.each(tokensPositionsArray, function (idx, tokenInfo) {
            result[tokenInfo.token_index] = tokenInfo;
            result[tokenInfo.token_index].finish_position = tokenInfo.start_position + tokenInfo.length;
        });
        return result;        
    }
    
    
    /* Fetching antiplagiarism status */
    $('.antiplagiarism-status').each(function () {
        let $self = $(this);
        let url = $self.data('antiplagiarismUrl');
        $.getJSON(url, function (data) {
            $self.addClass('found-level' + data.suspicion_level);
            let message = '';
            switch (data.suspicion_level)
            {
                case 0: message = 'похожих решений не найдено'; break;
                case 1: message = 'найдено {count} похожих решений других студентов. {link}'; break;
                case 2: message = 'найдено {count} похожих решений других студентов. {link}'; break;
            }
            message = message.replace('{count}', data.suspicious_submissions_count);
            message = message.replace('{link}', '<a href="' + $self.data('antiplagiarismDetailsUrl') + '" target="_blank">Посмотреть</a>');
            $self.html('Проверка на списывание: ' + message);
        });
    });
});