
; (function ($) {

    $.chatr = $.chatr || {};
    $.extend($.chatr, {
        init: function (options) {

            var t = $(this);
            if (t.data('chatr'))
                return;

            var o = $.extend({
                cuser: '',
                textTemplate: null
            }, options || {});

            t.data("chatr", o);

            var chat = $.connection.chatHub;

            chat.client.send = function (name, text) { _msg(name, o.cuser, text); };

            chat.client.online = function (user, usermeta) { _makeon(user, usermeta); };

            chat.client.setPairCount = function (user, count) { _changePairCount(user, count); }

            chat.client.pair = function (user) { _open(user, true); }

            chat.client.unpair = function (user) { _tryclose(user); }

            chat.client.keyboardSignal = function (user, target) { _keyboardSignal(user, target); }

            //chat.client.disconnected = function (user) { _makeoff(user); };

            $.connection.chatHub.connection.start().done(function () {
                if (o.cuser.length > 0) {
                    chat.server.notify(o.cuser, $.connection.hub.id,false);
                }
                chat.server.getOnlineUsers().done(function (users) {
                    for (var user in users)
                        chat.client.online(user, users[user]);
                });
            });

            $(window).resize(function () {
                _alignWins();
            });

            var _alignWins = function () {
                $('.chat-window-dialog:visible').each(function () {

                });
            }

            var _open = function (target, dont_shake) {

                var chatWindow = $('.chat-window.' + target);
                var msgbox = null;

                if (chatWindow.length == 0) {
                    chatWindow = o.textTemplate.clone();
                    chatWindow.removeClass('chat-window-template').addClass('chat-window ' + target);

                    var lastWin = $('.chat-window-dialog:last');
                    var at = lastWin.length > 0 ? "right bottom" : "left bottom";
                    var of = lastWin.length > 0 ? lastWin : $(window);

                    chatWindow.dialog({
                        closeOnEscape: true,
                        position: { my: "left bottom", at: at, of: of },
                        title: target,
                        height: 300,
                        width: 250,
                        maxHeight: 300,
                        maxWidth: 250,
                        draggable: false,
                        resizable: false,
                        show: {
                            effect: 'fade',
                            complete: function () {
                                _alignWins();

                                chat.server.pair(o.cuser, target);
                            }
                        },
                        dialogClass: 'chat-window-dialog',
                        close: function () {
                            chat.server.unpair(o.cuser, target);
                            _alignWins();
                            $(this).remove();
                        }
                    }).parent().css({
                        position: 'fixed'
                    }).attr('data-index', $('chat-window-dialog:visible').length);

                    _alignWins();

                    chatWindow.data('target-name', target);

                    msgbox = chatWindow.find('.message-box:first');

                    msgbox.on("keypress", function (e) {
                        if (e.which == 13) {
                            var txt = $(this);
                            var text = txt.val();
                            if (text.length == 0) {
                                txt.val('');
                                return;
                            }

                            var win = $(this).closest('.chat-window');
                            var target = win.data('target-name');
                            chat.server.sendToSpecific(o.cuser, target, text)
                                .done(function (result) {
                                    var msg = _msg(o.cuser, target, text);
                                    if (result) {
                                        msg.addClass("chat-not-delivery");
                                    }
                                });
                            txt.val('');
                            txt.focus();


                            e.preventDefault();
                        } else {

                            var cdate = new Date();
                            cdate.setSeconds(cdate.getSeconds() - 2);

                            if (!$(this).data('last-keyboard-signal') ||
                                $(this).data('last-keyboard-signal') < cdate) {

                                var win = $(this).closest('.chat-window');
                                var target = win.data('target-name');
                                chat.server.keyboardSignal(o.cuser, target);
                                $(this).data('last-keyboard-signal', new Date());
                            }
                        }

                    });

                    chatWindow
                        .find('.message-log')
                        .slimScroll({
                            height: '189px',
                            alwaysVisible: true,
                            position: 'left'
                        });

                } else {
                    if (!dont_shake)
                        chatWindow.parent().effect("shake", { times: 5 }, 160);
                    msgbox = chatWindow.find('message-box');
                }
                if (!chatWindow.is(':visible'))
                    chatWindow.dialog('open');

                msgbox.focus();

                return chatWindow;
            }

            var _getTime = function (time) {

                return time;
            }

            var _msg = function (sender, target, text, error) {
                var amISender = sender == o.cuser;
                if (amISender) sender = "من";

                var win = _open(amISender ? target : sender, true);
                var log = win.find('.message-log:first');
                var lastSender = log.find('li:last').find('.msg-sender:first').text().trim();


                if (lastSender != sender) {
                    var msg = o.textTemplate.find('.message-template:first').clone();
                    msg.find('.msg-sender:first').append(sender);
                    if (amISender)
                        msg.addClass('chat-me');
                    msg.removeClass('message-template');
                } else
                    msg = log.find('li:last');

                msg.find('.msg-text:first').append("<div>" + text + "</div>");
                msg.find('.msg-time:first').text(_getTime(new Date().toLocaleTimeString()));

                if (lastSender != sender) {
                    msg = msg.wrap('<li/>');
                    log.append(msg.show().parent());
                }
                var h = 0;
                log.children().each(function () {
                    h += $(this).height();
                });

                log.slimScroll({
                    scrollTo: h
                });

                if (!amISender) {
                    win.find('.chat-target-typing').hide('fade');
                }

                return msg;
            }

            var _changePairCount = function (user, value) {
                console.log("user : " + user + " value : " + value);
                var ul = $('.chat-online-users:first');
                var li = ul.find('.chat-user[name="' + user + '"]:first');
                if (li.length > 0) {
                    var pc = li.find('.chat-user-paircount:first');
                    pc.text(value.toString());
                }
            }

            var _tryclose = function (user) {
                var chatWindow = $('.chat-window.' + user);
                chatWindow.dialog('close');
            }

            var _keyboardSignal = function (user, target) {
                var chatWindow = $('.chat-window.' + user);
                var ts = chatWindow.find('.chat-target-typing');
                if (!ts.is(':visible'))
                    ts.show('fade');
                if (chatWindow.data('keyboard')) {
                    clearTimeout(chatWindow.data('keyboard'));
                    chatWindow.data('keyboard', null);
                }
                if (!chatWindow.data('keyboard'))
                    chatWindow.data('keyboard', setTimeout(function () {
                        if (ts.is(':visible'))
                            ts.hide('fade');
                    }, 5000));
            }

            var _makeon = function (user, metadata) {

                if (o.cuser == user) return;

                var ul = $('.chat-online-users:first');

                var li = ul.find('.chat-user[name="' + user + '"]:first');

                if (li.length == 0) {
                    li = ul.find('.chat-user-template:first')
                       .clone()
                       .removeClass("chat-user-template")
                       .addClass('chat-user')
                       .attr('name', user);

                    li.find('.chat-user-activation:first')
                          .click(function () {
                              var target = li.attr('name');
                              if (o.cuser.length == 0) {
                                  var nicname = prompt("لطفا نام خود را وارد کنید");
                                  o.cuser = nicname;
                                  if (o.cuser.length > 0) {
                                      chat.server
                                          .notify(o.cuser, $.connection.hub.id,true)
                                          .done(function (valid) {
                                              if (valid) {
                                                  _open(target);
                                              } else {
                                                  alert("نام انتخابی شما توسط فرد دیگری استفاده شده است، لطفا با نام دیگری مجدد سعی کنید.");
                                              }
                                          });

                                  }
                              } else
                                  _open(target);

                          });//a click

                    li.find('.chat-user-name').text(user);

                    for (var k in metadata)
                        li.find('.chat-user-' + k).text(metadata[k]);

                    ul.append(li);
                }
            }

        } // init


    });

    $.fn.extend({

        //This is where you write your plugin's name
        chatr: function (pin) {
            var _this = this;

            if (typeof pin == 'string') {
                var fn = $.chatr[pin];
                if (!fn) {
                    throw ("chatr - No such method: " + pin);
                }
                var args = $.makeArray(arguments).slice(1);
                return fn.apply(this, args);
            } else if (typeof pin === 'object' || !pin) {
                return $.chatr.init.apply(this, arguments);
            }
            else {
                $.error('Method ' + pin + ' does not exist on jQuery.chatr');
            }
        }
    });
})(jQuery);