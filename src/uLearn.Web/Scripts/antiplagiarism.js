function fetchAntiPlagiarismStatus($plagiarismStatus) {
    $plagiarismStatus.removeClass('found-level0 found-level1 found-level2');
    
    var url = $plagiarismStatus.data('antiplagiarismUrl');
    $.getJSON(url, function (data) {
        $plagiarismStatus.addClass('found-level' + data.suspicion_level);
        var message = '';
        switch (data.suspicion_level)
        {
            case 0: message = 'похожих решений не найдено'; break;
            case 1:
            case 2:
                var singleNumberMessage = 'найдено похожее решение у {count} другого студента. {link}';
                var pluralNumberMessage = 'найдены похожие решения у {count} других студентов. {link}';
                message = data.suspicious_authors_count === 1 ? singleNumberMessage : pluralNumberMessage;
                break;
        }
        message = message.replace('{count}', data.suspicious_authors_count);
        message = message.replace('{link}', '<a href="' + $plagiarismStatus.data('antiplagiarismDetailsUrl') + '" target="_blank">Посмотреть</a>');
        
        $plagiarismStatus.html('Проверка на списывание: ' + message);
    });
}

$(document).ready(function () {
    $('.antiplagiarism__data').each(function () {
        var $self = $(this);
        var originalSubmissionId = $self.data('originalSubmissionId');
        var plagiarismSubmissionId = $self.data('plagiarismSubmissionId');

        var $originalSubmission = $('.code[data-submission-id="' + originalSubmissionId + '"]');
        var $plagiarismSubmission = $('.code[data-submission-id="' + plagiarismSubmissionId + '"]');
        var originalCodeMirror = $originalSubmission[0].codeMirrorEditor;
        var plagiarismCodeMirror = $plagiarismSubmission[0].codeMirrorEditor;
        
        var antiplagiarismData = JSON.parse($self[0].innerHTML);       
        var plagiarismData = antiplagiarismData.plagiarism;

        var originalTokens = getTokensDictionaryByIndex(antiplagiarismData.tokens_positions);
        var plagiarismTokens = getTokensDictionaryByIndex(plagiarismData.tokens_positions);
       
        /* Batch all operations as one: see https://codemirror.net/doc/manual.html for details. It's much faster because
         * doesn't need to fully relayout and redraw DOM tree */
        originalCodeMirror.operation(function () {
            highlightNotAnalyzedParts(originalCodeMirror, antiplagiarismData.analyzed_code_units, originalTokens);    
        });
        plagiarismCodeMirror.operation(function() {
            highlightNotAnalyzedParts(plagiarismCodeMirror, plagiarismData.analyzed_code_units, plagiarismTokens);    
        });        
        
        highlightMatchedTokens(plagiarismData.matched_snippets, originalCodeMirror, plagiarismCodeMirror, originalTokens, plagiarismTokens);
    });

    function getRandomColor() {
        // 30 random hues with step of 12 degrees
        var hue = Math.floor(Math.random() * 30) * 12;

        return $.Color({
            hue: hue,
            saturation: 0.9,
            lightness: 0.9,
            alpha: 0.5
        }).toRgbaString();
    }
    
    function getTokensDictionaryByIndex(tokensPositionsArray) {
        var result = {};
        $.each(tokensPositionsArray, function (idx, tokenInfo) {
            result[tokenInfo.token_index] = tokenInfo;
            result[tokenInfo.token_index].finish_position = tokenInfo.start_position + tokenInfo.length;
        });
        return result;        
    }
    
    function highlightNotAnalyzedParts(codeMirrorEditor, analyzedCodeUnits, tokens) {
        var document = codeMirrorEditor.getDoc();

        var highlightedTokes = [];
        $.each(analyzedCodeUnits, function (idx, codeUnit) {
            var firstTokenIndex = codeUnit.first_token_index;
            var lastTokenIndex = codeUnit.first_token_index + codeUnit.tokens_count - 1;
            for (var tokenIndex = firstTokenIndex; tokenIndex <= lastTokenIndex; tokenIndex++)
                highlightedTokes.push(tokenIndex);
        });
        highlightedTokes.sort(function (a, b) {
            return a - b;
        });
        
        var textMarkerOptions = {
            className: 'antiplagiarism__not-analyzed',
            title: 'Эта часть кода не анализируется на списывание',
        };

        var currentHighlightStart = 0;
        for (var idx = 0; idx < highlightedTokes.length; idx++) {
            if (idx === 0 || highlightedTokes[idx - 1] < highlightedTokes[idx] - 1) {
                var currentHighlightFinish = tokens[highlightedTokes[idx]].start_position;
                if (currentHighlightStart !== currentHighlightFinish) {  
                    document.markText(
                        document.posFromIndex(currentHighlightStart),
                        document.posFromIndex(currentHighlightFinish),
                        textMarkerOptions
                    );
                }
            }
            currentHighlightStart = tokens[highlightedTokes[idx]].finish_position;
        }

        document.markText(document.posFromIndex(currentHighlightStart), document.posFromIndex(1e10), textMarkerOptions);
    }    

    function highlightMatchedTokens(matchedSnippets, originalCodeMirror, plagiarismCodeMirror, originalTokens, plagiarismTokens) {
        /* Batch all operations as one: see https://codemirror.net/doc/manual.html for details */
        originalCodeMirror.operation(function () {
            highlightMatchedTokensInSubmission(matchedSnippets, originalCodeMirror, originalTokens, 'original_submission_first_token_index');    
        });
        plagiarismCodeMirror.operation(function () {
            highlightMatchedTokensInSubmission(matchedSnippets, plagiarismCodeMirror, plagiarismTokens, 'plagiarism_submission_first_token_index');    
        });        
    }
    
    function highlightMatchedTokensInSubmission(matchedSnippets, codeMirrorEditor, tokens, firstTokenIndexSelector) {
        var tokensPlagiarismTypes = {};
        var maxTokenIndex = 0;
        $.each(matchedSnippets, function (idx, matchedSnippet) {
            var snippetType = matchedSnippet.snippet_type;
            for (var tokenIndex = matchedSnippet[firstTokenIndexSelector];
                 tokenIndex < matchedSnippet[firstTokenIndexSelector] + matchedSnippet.snippet_tokens_count;
                 tokenIndex++) {
                var oldValue = tokensPlagiarismTypes[tokenIndex];
                var newValue = snippetType;
                if (oldValue === undefined || (newValue === 'tokensKindsAndValues'))
                    tokensPlagiarismTypes[tokenIndex] = newValue;
                
                if (tokenIndex > maxTokenIndex)
                    maxTokenIndex = tokenIndex;
            }
        });

        var document = codeMirrorEditor.getDoc();
        var currentStart = 0, currentFinish = 0, currentPlagiarismType = '';

        var hightlightCurrentTokensSequence = function() {
            document.markText(
                document.posFromIndex(currentStart),
                document.posFromIndex(currentFinish),
                {
                    className: 'antiplagiarism__plagiarism-token-' + currentPlagiarismType,
                    title: currentPlagiarismType === 'tokensKindsAndValues'
                        ? 'Эта часть кода совпадает полностью' 
                        : 'Эта часть кода подозрительно похожа'
                }
            );
        };
        
        for (var tokenIndex = 0; tokenIndex <= maxTokenIndex; tokenIndex++) {
            if (! (tokenIndex in tokensPlagiarismTypes)) {
                hightlightCurrentTokensSequence();
            } else if (! (tokenIndex - 1 in tokensPlagiarismTypes)) {
                currentStart = tokens[tokenIndex].start_position;
                currentPlagiarismType = tokensPlagiarismTypes[tokenIndex];
            } else if (tokensPlagiarismTypes[tokenIndex] !== tokensPlagiarismTypes[tokenIndex - 1]) {
                hightlightCurrentTokensSequence();
                currentStart = tokens[tokenIndex].start_position;
                currentPlagiarismType = tokensPlagiarismTypes[tokenIndex];
            }
            
            if (tokenIndex in tokensPlagiarismTypes)
                currentFinish = tokens[tokenIndex].finish_position;
        }        

        hightlightCurrentTokensSequence();        
    }
      
    /* Fetching antiplagiarism status */
    $('.antiplagiarism-status').each(function () {
        fetchAntiPlagiarismStatus($(this));
    });
    
    /* Changing submission on panel */
    $('.antiplagiarism__submissions-panel [name="submissionId"]').change(function () {
        var $self = $(this);
        $('.antiplagiarism').hide();
        var $form = $self.closest('form');        
        $form.submit();
    });
});