$(function() {
    $('.shape').each(function() {
        //var color = ((1 << 24) * (Math.random() + 1) | 0).toString(16).substr(1);
        var color = '#' +
                    Math.floor((Math.random() * 4 + 8)).toString(16) + Math.floor((Math.random() * 15)).toString(16) +
                    Math.floor((Math.random() * 4 + 8)).toString(16) + Math.floor((Math.random() * 15)).toString(16) +
                    Math.floor((Math.random() * 4 + 8)).toString(16) + Math.floor((Math.random() * 15)).toString(16);

        $(this).css('background-color', '#' + color);
    });

    $(document).on('click', '.launch-btn', function() {
        $('.rating').fadeIn();
    });

    var valuesHash = {};

    $(document).on('click', '.details', function() {
        $('.details-box').each(function() {
            var thisId = $(this).data('id');

            $(this).find('ul li').each(function() {
                var instanceId = $(this).data('id');

                $(this).children('span:first').text(getValueFromHash(instanceId, thisId));
            });
        });
        $('.details-content').slideDown('slow');
    });

    $(document).on('click', '.details-content .btn', function() {
        $('.details-content').slideUp('slow');
    });

    $(document).on('click', '.bubble .shape', function() {
        var info = $(this).children('.info');

        info.hide();
        $('.activeItem').css('background-color', $(this).css('background-color'))
                        .text(info.text()).show();

        var bubble = $(this).parent('.bubble');

        if (bubble.hasClass(('active-bubble'))) {
            return;
        }

        $('.shape > .info').hide();
        $('.bubble').removeClass('unset active-bubble bubble-bottom bubble-top bubble-right bubble-left');

        $('.rangeSlider').fadeOut('slow', function() {
            $(this).remove();
        });

        bubble.addClass('active-bubble')
              .css({ position: 'absolute' })
              .animate({ top: '43%', left: '48%' }, {
                  duration: 1000, complete: function() {
                      var angle = getAngle();
                      var lengthOfSlider = getLengthOfSliderInPixels();
                      var centerPoint = $(this).offset();
                      var exceptThis = $('.bubble:not(".active-bubble")');
                      var shape = $(this).children('.shape');
                      var shapeOffset = shape.offset();
                      var shapeWidth = shape.width();
                      var shapeHeightMiddle = shape.height() / 2;
                      var transformation = '-' + (shapeWidth / 2) + 'px 50% 0';
                      var constant = 4;//(Object.hasOwnProperty.call(window, "ActiveXObject") && !window.ActiveXObject) ? 27 : 4;

                      generateSliders(exceptThis, this.id);

                      $('.rangeSlider').css({
                          width: lengthOfSlider - shapeWidth + 'px',
                          left: shapeOffset.left + shapeWidth - centerPoint.left + 'px',
                          top: shapeOffset.top + shapeHeightMiddle - centerPoint.top - constant + 'px',
                          '-moz-transform-origin': transformation,
                          '-ms-transform-origin': transformation,
                          '-o-transform-origin': transformation,
                          '-webkit-transform-origin': transformation,
                          'transform-origin': transformation
                      });

                      exceptThis.each(function(i) {
                          var thisAngle = angle * i;
                          var location = getCoordinatesInPercentageWithUnit(thisAngle, lengthOfSlider, centerPoint);
                          var rotation = 'rotate(' + thisAngle + 'deg)';
                          console.warn(rotation);
                          $('.rangeSlider').eq(i).css({
                              '-webkit-transform': rotation,
                              '-moz-transform': rotation,
                              '-o-transform': rotation,
                              '-ms-transform': rotation,
                              'transform': rotation
                          });

                          $(this).css({ position: 'absolute' })
                                 .addClass(thisAngle === 90 ? 'bubble-bottom' :
                                          (thisAngle === 270) ? 'bubble-top' :
                                          ((thisAngle >= 0 && thisAngle < 90) || (thisAngle > 270 && thisAngle < 360)) ? 'bubble-right' :
                                          (thisAngle > 90 && thisAngle < 270) ? 'bubble-left' : "")
                                 .animate({ top: location.top, left: location.left }, {
                                     duration: 1000, queue: false, complete: function() {
                                         if (!$('.rangeSlider').is(':visible')) {
                                             $('.rangeSlider').fadeIn('slow');
                                             $('.shape > .info').not(info).fadeIn('slow');
                                         }
                                     }
                                 });
                      });
                  }
              });
    });

    function generateSliders(bubbles, currentId) {
        bubbles.each(function(i) {
            var thisId = bubbles[i].id;
            $('<input class="rangeSlider" style="display:none" type="range" value="' + getValueFromHash(thisId, currentId) + '" id="' + thisId + '" step="1" min="0" max="' + scale + '" />').appendTo($('.bubble.active-bubble'));
        });
    }

    $(document).on('change', '.rangeSlider', function() {
        var parentId = $(this).parent().attr('id');
        var thisId = $(this).attr('id');

        if (parentId in valuesHash && (!valuesHash[thisId] || !(parentId in valuesHash[thisId]))) {
            valuesHash[parentId][thisId] = $(this).val();
        } else if (thisId in valuesHash && (!valuesHash[parentId] || !(thisId in valuesHash[parentId]))) {
            valuesHash[thisId][parentId] = scale - $(this).val();
        } else {
            valuesHash[parentId] = {};
            valuesHash[parentId][thisId] = $(this).val();
        }

        updateProgress();
    });

    function getValueFromHash(instanceId, currentId) {
        if (currentId in valuesHash && instanceId in valuesHash[currentId]) {
            return valuesHash[currentId][instanceId];
        } else if (instanceId in valuesHash && currentId in valuesHash[instanceId]) {
            return scale - valuesHash[instanceId][currentId];
        }

        return 0;
    }

    function getCoordinatesInPercentageWithUnit(angle, heightOfSlider, centerPoint) {
        var edges = getCoordinatesOfTriangle(angle, heightOfSlider);
        var newTop = centerPoint.top + edges.opposite;
        var newLeft = centerPoint.left + edges.adjacent;

        return {
            top: newTop + 'px',
            left: newLeft + 'px'
        };
        //return convertPixelToPercentageWithUnit({ top: newTop, left: newLeft });
    }

    function getCoordinatesOfTriangle(angle, heightOfSlider) {
        var angleInRadians = angle * Math.PI / 180;
        return {
            opposite: Math.sin(angleInRadians) * heightOfSlider,
            adjacent: Math.cos(angleInRadians) * heightOfSlider
        };
    }

    function getAngle() {
        var totalAttributes = getTotalAttributes();
        return 360 / (totalAttributes - 1);
    }

    function getTotalAttributes() {
        return $('.shape').length;
    }

    function getLengthOfSliderInPixels() {
        return ($('.rating-content').height() - ($('.bubble').height())) / 2;
    }

    function convertPixelToPercentageWithUnit(value) {
        return {
            left: ((value.left / screen.width) * 100) + '%',
            top: ((value.top / screen.height) * 100) + '%'
        };
    }

    function updateProgress() {
        var length = $('.bubble').length;
        var combinations = factorial(length) / (2 * factorial(length - 2));
        var countAttempts = 0;

        for (var key in valuesHash) {
            countAttempts += Object.keys(valuesHash[key]).length;
        }

        var calculatedProgress = Math.round(100 - (100 * (combinations - countAttempts) / combinations));

        setProgress(calculatedProgress);

        return calculatedProgress;
    }

    function factorial(num) {
        var rval = 1;

        for (var i = 2; i <= num; i++) {
            rval = rval * i;
        }

        return rval;
    }

    function setProgress(width) {
        var widthWithUnit = width + '%';
        $('button[type=submit]').prop('disabled', width < 50);
        $('.progress').css({ width: widthWithUnit });
        $('.counter').text(widthWithUnit);
    }

    $('.rating-form').submit(function() {
        if (updateProgress() < 50) {
            return false;
        }

        var i = 0;
        var evaluationId = $("#EvaluationId").val();
        var fieldset = "";

        for (var key1 in valuesHash) {
            for (var key2 in valuesHash[key1]) {
                fieldset +=
                 '<input name="[' + i + '].Value1" value="' + valuesHash[key1][key2] + '" type="hidden" /> \
                  <input name="[' + i + '].Value2" value="' + valuesHash[key1][key2] + '" type="hidden" /> \
                  <input name="[' + i + '].AttributeId1" value="' + key1 + '" type="hidden" /> \
                  <input name="[' + i + '].AttributeId2" value="' + key2 + '" type="hidden" /> \
                  <input name="[' + i + '].EvaluationUserId" value="' + evaluationId + '" type="hidden" />';
                ++i;
            }
        }

        $(fieldset).appendTo($(this));
    });
});
