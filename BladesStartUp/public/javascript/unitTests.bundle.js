/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// identity function for calling harmony imports with the correct context
/******/ 	__webpack_require__.i = function(value) { return value; };
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, {
/******/ 				configurable: false,
/******/ 				enumerable: true,
/******/ 				get: getter
/******/ 			});
/******/ 		}
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = 16);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
//date parse method from https://weblog.west-wind.com/posts/2014/jan/06/javascript-json-date-parsing-and-real-dates
var reISO = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*))(?:Z|(\+|-)([\d|:]*))?$/;
var reMsAjax = /^\/Date\((d|-|.*)\)[\/|\\]$/;
var parseMsAjaxDate = false;
var dateParser = function (key, value) {
    /// <summary>
    /// Globally enables JSON date parsing for JSON.parse().
    /// Replaces the default JSON.parse() method and adds
    /// the datePaser() extension to the processing chain.
    /// </summary>    
    /// <param name="key" type="string">property name that is parsed</param>
    /// <param name="value" type="any">property value</param>
    /// <returns type="date">returns date or the original value if not a date string</returns>
    if (typeof value === 'string') {
        var a = reISO.exec(value);
        if (a)
            return new Date(value);
        if (!parseMsAjaxDate)
            return value;
        a = reMsAjax.exec(value);
        if (a) {
            var b = a[1].split(/[-+,.]/);
            return new Date(b[0] ? +b[0] : 0 - +b[1]);
        }
    }
    return value;
};
var json = {
    parse: function (str) {
        try {
            var res = JSON.parse(str, dateParser);
            return res;
        }
        catch (e) {
            // orignal error thrown has no error message so rethrow with message
            throw new Error("JSON content could not be parsed");
        }
    },
    stringify: function (data) {
        return JSON.stringify(data);
    }
};
exports.default = json;


/***/ }),
/* 1 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var noop = function () { };
exports.default = noop;


/***/ }),
/* 2 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var json_1 = __webpack_require__(0);
var noop_1 = __webpack_require__(1);
var ServerConnection = (function () {
    function ServerConnection() {
        this.connectionProtocol = 'ws'; //or wss
        this.connectionRoute = 'ws';
        this.reconnectTimeout = 15000;
        this.reconnectIntervalRef = 0;
        this.sendMessagesTimeout = 100;
        this.sendMessagesIntervalRef = 0;
        this.messagesQueue = [];
        this.rawMessageHandlers = [];
        this.notifyMessageHandlers = {};
    }
    ServerConnection.prototype.open = function () {
        var _this = this;
        if (this.isOpen) {
            return;
        }
        this.connect();
        clearInterval(this.reconnectIntervalRef);
        this.reconnectIntervalRef = setInterval(function () {
            if (!_this.ws || _this.ws.readyState !== WebSocket.OPEN) {
                _this.connect();
            }
        }, this.reconnectTimeout);
    };
    ServerConnection.prototype.close = function () {
        if (this.ws) {
            this.ws.close();
        }
        clearInterval(this.reconnectIntervalRef);
    };
    Object.defineProperty(ServerConnection.prototype, "isOpen", {
        get: function () {
            if (!this.ws) {
                return false;
            }
            return this.ws.readyState === WebSocket.OPEN;
        },
        enumerable: true,
        configurable: true
    });
    ServerConnection.prototype.connect = function () {
        var _this = this;
        this.ws = new WebSocket(this.connectionProtocol + "://" + document.location.host + "/" + this.connectionRoute);
        this.bindEvents();
        clearInterval(this.sendMessagesIntervalRef);
        this.sendMessagesIntervalRef = setInterval(function () {
            _this.sendMessagesFromQueue();
        }, this.sendMessagesTimeout);
    };
    ServerConnection.prototype.bindEvents = function () {
        var _this = this;
        this.ws.onclose = function () {
            clearInterval(_this.sendMessagesIntervalRef);
        };
        this.ws.onmessage = function (ev) {
            _this.rawMessage(ev);
            _this.processNotifyMessage(ev);
        };
    };
    ServerConnection.prototype.sendMessagesFromQueue = function () {
        while (this.messagesQueue.length > 0) {
            if (this.ws.readyState !== WebSocket.OPEN) {
                break;
            }
            var data = this.messagesQueue.shift();
            this.ws.send(data);
        }
    };
    ServerConnection.prototype.send = function (data) {
        this.messagesQueue.push(data);
    };
    ServerConnection.prototype.rawMessage = function (e) {
        this.rawMessageHandlers.forEach(function (handler) { return handler(e); });
    };
    ServerConnection.prototype.onRawMessage = function (handler) {
        var _this = this;
        if (!handler) {
            return noop_1.default;
        }
        this.rawMessageHandlers.push(handler);
        return function () {
            var index = _this.rawMessageHandlers.indexOf(handler);
            if (index > -1) {
                _this.rawMessageHandlers.splice(index, 1);
            }
        };
    };
    ServerConnection.prototype.processNotifyMessage = function (e) {
        try {
            var message_1 = json_1.default.parse(e.data);
            if (message_1.messageRole === 'NotifyMessage'
                && message_1.name
                && this.notifyMessageHandlers[message_1.name]) {
                this.notifyMessageHandlers[message_1.name]
                    .forEach(function (handler) { return handler(message_1.data, message_1.attributes); });
            }
        }
        catch (e) {
            //ignore
        }
    };
    ServerConnection.prototype.onNotifyMessage = function (name, handler) {
        var _this = this;
        if (!(name && handler)) {
            return noop_1.default;
        }
        if (!this.notifyMessageHandlers[name]) {
            this.notifyMessageHandlers[name] = [];
        }
        this.notifyMessageHandlers[name].push(handler);
        return function () {
            var index = _this.notifyMessageHandlers[name].indexOf(handler);
            if (index > -1) {
                _this.notifyMessageHandlers[name].splice(index, 1);
            }
        };
    };
    return ServerConnection;
}());
exports.ServerConnection = ServerConnection;
var connection = new ServerConnection();
exports.default = connection;


/***/ }),
/* 3 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var es6_promise_1 = __webpack_require__(6);
var json_1 = __webpack_require__(0);
var RequestExecutionError_1 = __webpack_require__(10);
var Xhr = (function () {
    function Xhr(url, method, body) {
        if (method === void 0) { method = 'GET'; }
        this.xhr = new XMLHttpRequest();
        this.reqUrl = url;
        this.reqMethod = method;
        this.reqBody = body;
        this.xhr.open(this.reqMethod, this.reqUrl, true);
    }
    Object.defineProperty(Xhr.prototype, "innerXhr", {
        get: function () {
            return this.xhr;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Xhr.prototype, "status", {
        get: function () {
            return this.xhr.status;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Xhr.prototype, "statusText", {
        get: function () {
            return this.xhr.statusText;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Xhr.prototype, "response", {
        get: function () {
            return this.xhr.response;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Xhr.prototype, "responseText", {
        get: function () {
            return this.xhr.responseText;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Xhr.prototype, "readyState", {
        get: function () {
            return this.xhr.readyState;
        },
        enumerable: true,
        configurable: true
    });
    Xhr.prototype.setRequestHeader = function (header, value) {
        this.xhr.setRequestHeader(header, value);
    };
    Xhr.prototype.rawExecute = function () {
        var _this = this;
        return new es6_promise_1.Promise(function (res, rej) {
            _this.xhr.send(_this.reqBody);
            _this.xhr.onreadystatechange = (function () {
                if (_this.xhr.readyState !== XMLHttpRequest.DONE) {
                    return;
                }
                res(_this);
            });
            _this.xhr.onerror = (function (ev) {
                rej(_this);
            });
        });
    };
    Xhr.beforeExecution = function (hook) {
        var next = Xhr.beforeExecutionChain;
        Xhr.beforeExecutionChain = (function (xhr) { return hook(xhr).then(next); });
    };
    Xhr.afterExecution = function (hook) {
        var next = Xhr.afterExecutionChain;
        Xhr.afterExecutionChain = (function (xhr) { return hook(xhr).then(next); });
    };
    Xhr.createResData = function (xhr) {
        if (xhr.readyState !== XMLHttpRequest.DONE) {
            throw new Error('Try to parse XHR in incorrect state');
        }
        var data = json_1.default.parse(xhr.responseText);
        return data;
    };
    Xhr.prototype.execute = function () {
        var _this = this;
        return new es6_promise_1.Promise(function (res, rej) {
            Xhr.beforeExecutionChain(_this)
                .then(function (xhr) { return xhr.rawExecute(); })
                .then(function (xhr) { return Xhr.afterExecutionChain(xhr); })
                .then(function (xhr) {
                if (xhr.status !== 200) {
                    rej(new RequestExecutionError_1.default(xhr));
                    return;
                }
                var data = Xhr.createResData(xhr);
                res(data);
            })
                .catch(function (err) { return rej(new RequestExecutionError_1.default(err)); });
        });
    };
    return Xhr;
}());
Xhr.beforeExecutionChain = function (xhr) { return new es6_promise_1.Promise(function (res) { return res(xhr); }); };
Xhr.afterExecutionChain = function (xhr) { return new es6_promise_1.Promise(function (res) { return res(xhr); }); };
exports.default = Xhr;


/***/ }),
/* 4 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
__webpack_require__(8);
__webpack_require__(14);


/***/ }),
/* 5 */
/***/ (function(module, exports) {

describe('Jasmine own tests', function () {
    it('True to be true', function () {
        var a = true;
        expect(a).toBe(true);
    });
    it('1 to be 1 and 2 to be 2', function () {
        expect(1).toBe(1);
        expect(2).toBe(2);
    });
});


/***/ }),
/* 6 */
/***/ (function(module, exports, __webpack_require__) {

/* WEBPACK VAR INJECTION */(function(process, global) {var require;/*!
 * @overview es6-promise - a tiny implementation of Promises/A+.
 * @copyright Copyright (c) 2014 Yehuda Katz, Tom Dale, Stefan Penner and contributors (Conversion to ES6 API by Jake Archibald)
 * @license   Licensed under MIT license
 *            See https://raw.githubusercontent.com/stefanpenner/es6-promise/master/LICENSE
 * @version   4.1.0
 */

(function (global, factory) {
     true ? module.exports = factory() :
    typeof define === 'function' && define.amd ? define(factory) :
    (global.ES6Promise = factory());
}(this, (function () { 'use strict';

function objectOrFunction(x) {
  return typeof x === 'function' || typeof x === 'object' && x !== null;
}

function isFunction(x) {
  return typeof x === 'function';
}

var _isArray = undefined;
if (!Array.isArray) {
  _isArray = function (x) {
    return Object.prototype.toString.call(x) === '[object Array]';
  };
} else {
  _isArray = Array.isArray;
}

var isArray = _isArray;

var len = 0;
var vertxNext = undefined;
var customSchedulerFn = undefined;

var asap = function asap(callback, arg) {
  queue[len] = callback;
  queue[len + 1] = arg;
  len += 2;
  if (len === 2) {
    // If len is 2, that means that we need to schedule an async flush.
    // If additional callbacks are queued before the queue is flushed, they
    // will be processed by this flush that we are scheduling.
    if (customSchedulerFn) {
      customSchedulerFn(flush);
    } else {
      scheduleFlush();
    }
  }
};

function setScheduler(scheduleFn) {
  customSchedulerFn = scheduleFn;
}

function setAsap(asapFn) {
  asap = asapFn;
}

var browserWindow = typeof window !== 'undefined' ? window : undefined;
var browserGlobal = browserWindow || {};
var BrowserMutationObserver = browserGlobal.MutationObserver || browserGlobal.WebKitMutationObserver;
var isNode = typeof self === 'undefined' && typeof process !== 'undefined' && ({}).toString.call(process) === '[object process]';

// test for web worker but not in IE10
var isWorker = typeof Uint8ClampedArray !== 'undefined' && typeof importScripts !== 'undefined' && typeof MessageChannel !== 'undefined';

// node
function useNextTick() {
  // node version 0.10.x displays a deprecation warning when nextTick is used recursively
  // see https://github.com/cujojs/when/issues/410 for details
  return function () {
    return process.nextTick(flush);
  };
}

// vertx
function useVertxTimer() {
  if (typeof vertxNext !== 'undefined') {
    return function () {
      vertxNext(flush);
    };
  }

  return useSetTimeout();
}

function useMutationObserver() {
  var iterations = 0;
  var observer = new BrowserMutationObserver(flush);
  var node = document.createTextNode('');
  observer.observe(node, { characterData: true });

  return function () {
    node.data = iterations = ++iterations % 2;
  };
}

// web worker
function useMessageChannel() {
  var channel = new MessageChannel();
  channel.port1.onmessage = flush;
  return function () {
    return channel.port2.postMessage(0);
  };
}

function useSetTimeout() {
  // Store setTimeout reference so es6-promise will be unaffected by
  // other code modifying setTimeout (like sinon.useFakeTimers())
  var globalSetTimeout = setTimeout;
  return function () {
    return globalSetTimeout(flush, 1);
  };
}

var queue = new Array(1000);
function flush() {
  for (var i = 0; i < len; i += 2) {
    var callback = queue[i];
    var arg = queue[i + 1];

    callback(arg);

    queue[i] = undefined;
    queue[i + 1] = undefined;
  }

  len = 0;
}

function attemptVertx() {
  try {
    var r = require;
    var vertx = __webpack_require__(19);
    vertxNext = vertx.runOnLoop || vertx.runOnContext;
    return useVertxTimer();
  } catch (e) {
    return useSetTimeout();
  }
}

var scheduleFlush = undefined;
// Decide what async method to use to triggering processing of queued callbacks:
if (isNode) {
  scheduleFlush = useNextTick();
} else if (BrowserMutationObserver) {
  scheduleFlush = useMutationObserver();
} else if (isWorker) {
  scheduleFlush = useMessageChannel();
} else if (browserWindow === undefined && "function" === 'function') {
  scheduleFlush = attemptVertx();
} else {
  scheduleFlush = useSetTimeout();
}

function then(onFulfillment, onRejection) {
  var _arguments = arguments;

  var parent = this;

  var child = new this.constructor(noop);

  if (child[PROMISE_ID] === undefined) {
    makePromise(child);
  }

  var _state = parent._state;

  if (_state) {
    (function () {
      var callback = _arguments[_state - 1];
      asap(function () {
        return invokeCallback(_state, child, callback, parent._result);
      });
    })();
  } else {
    subscribe(parent, child, onFulfillment, onRejection);
  }

  return child;
}

/**
  `Promise.resolve` returns a promise that will become resolved with the
  passed `value`. It is shorthand for the following:

  ```javascript
  let promise = new Promise(function(resolve, reject){
    resolve(1);
  });

  promise.then(function(value){
    // value === 1
  });
  ```

  Instead of writing the above, your code now simply becomes the following:

  ```javascript
  let promise = Promise.resolve(1);

  promise.then(function(value){
    // value === 1
  });
  ```

  @method resolve
  @static
  @param {Any} value value that the returned promise will be resolved with
  Useful for tooling.
  @return {Promise} a promise that will become fulfilled with the given
  `value`
*/
function resolve(object) {
  /*jshint validthis:true */
  var Constructor = this;

  if (object && typeof object === 'object' && object.constructor === Constructor) {
    return object;
  }

  var promise = new Constructor(noop);
  _resolve(promise, object);
  return promise;
}

var PROMISE_ID = Math.random().toString(36).substring(16);

function noop() {}

var PENDING = void 0;
var FULFILLED = 1;
var REJECTED = 2;

var GET_THEN_ERROR = new ErrorObject();

function selfFulfillment() {
  return new TypeError("You cannot resolve a promise with itself");
}

function cannotReturnOwn() {
  return new TypeError('A promises callback cannot return that same promise.');
}

function getThen(promise) {
  try {
    return promise.then;
  } catch (error) {
    GET_THEN_ERROR.error = error;
    return GET_THEN_ERROR;
  }
}

function tryThen(then, value, fulfillmentHandler, rejectionHandler) {
  try {
    then.call(value, fulfillmentHandler, rejectionHandler);
  } catch (e) {
    return e;
  }
}

function handleForeignThenable(promise, thenable, then) {
  asap(function (promise) {
    var sealed = false;
    var error = tryThen(then, thenable, function (value) {
      if (sealed) {
        return;
      }
      sealed = true;
      if (thenable !== value) {
        _resolve(promise, value);
      } else {
        fulfill(promise, value);
      }
    }, function (reason) {
      if (sealed) {
        return;
      }
      sealed = true;

      _reject(promise, reason);
    }, 'Settle: ' + (promise._label || ' unknown promise'));

    if (!sealed && error) {
      sealed = true;
      _reject(promise, error);
    }
  }, promise);
}

function handleOwnThenable(promise, thenable) {
  if (thenable._state === FULFILLED) {
    fulfill(promise, thenable._result);
  } else if (thenable._state === REJECTED) {
    _reject(promise, thenable._result);
  } else {
    subscribe(thenable, undefined, function (value) {
      return _resolve(promise, value);
    }, function (reason) {
      return _reject(promise, reason);
    });
  }
}

function handleMaybeThenable(promise, maybeThenable, then$$) {
  if (maybeThenable.constructor === promise.constructor && then$$ === then && maybeThenable.constructor.resolve === resolve) {
    handleOwnThenable(promise, maybeThenable);
  } else {
    if (then$$ === GET_THEN_ERROR) {
      _reject(promise, GET_THEN_ERROR.error);
      GET_THEN_ERROR.error = null;
    } else if (then$$ === undefined) {
      fulfill(promise, maybeThenable);
    } else if (isFunction(then$$)) {
      handleForeignThenable(promise, maybeThenable, then$$);
    } else {
      fulfill(promise, maybeThenable);
    }
  }
}

function _resolve(promise, value) {
  if (promise === value) {
    _reject(promise, selfFulfillment());
  } else if (objectOrFunction(value)) {
    handleMaybeThenable(promise, value, getThen(value));
  } else {
    fulfill(promise, value);
  }
}

function publishRejection(promise) {
  if (promise._onerror) {
    promise._onerror(promise._result);
  }

  publish(promise);
}

function fulfill(promise, value) {
  if (promise._state !== PENDING) {
    return;
  }

  promise._result = value;
  promise._state = FULFILLED;

  if (promise._subscribers.length !== 0) {
    asap(publish, promise);
  }
}

function _reject(promise, reason) {
  if (promise._state !== PENDING) {
    return;
  }
  promise._state = REJECTED;
  promise._result = reason;

  asap(publishRejection, promise);
}

function subscribe(parent, child, onFulfillment, onRejection) {
  var _subscribers = parent._subscribers;
  var length = _subscribers.length;

  parent._onerror = null;

  _subscribers[length] = child;
  _subscribers[length + FULFILLED] = onFulfillment;
  _subscribers[length + REJECTED] = onRejection;

  if (length === 0 && parent._state) {
    asap(publish, parent);
  }
}

function publish(promise) {
  var subscribers = promise._subscribers;
  var settled = promise._state;

  if (subscribers.length === 0) {
    return;
  }

  var child = undefined,
      callback = undefined,
      detail = promise._result;

  for (var i = 0; i < subscribers.length; i += 3) {
    child = subscribers[i];
    callback = subscribers[i + settled];

    if (child) {
      invokeCallback(settled, child, callback, detail);
    } else {
      callback(detail);
    }
  }

  promise._subscribers.length = 0;
}

function ErrorObject() {
  this.error = null;
}

var TRY_CATCH_ERROR = new ErrorObject();

function tryCatch(callback, detail) {
  try {
    return callback(detail);
  } catch (e) {
    TRY_CATCH_ERROR.error = e;
    return TRY_CATCH_ERROR;
  }
}

function invokeCallback(settled, promise, callback, detail) {
  var hasCallback = isFunction(callback),
      value = undefined,
      error = undefined,
      succeeded = undefined,
      failed = undefined;

  if (hasCallback) {
    value = tryCatch(callback, detail);

    if (value === TRY_CATCH_ERROR) {
      failed = true;
      error = value.error;
      value.error = null;
    } else {
      succeeded = true;
    }

    if (promise === value) {
      _reject(promise, cannotReturnOwn());
      return;
    }
  } else {
    value = detail;
    succeeded = true;
  }

  if (promise._state !== PENDING) {
    // noop
  } else if (hasCallback && succeeded) {
      _resolve(promise, value);
    } else if (failed) {
      _reject(promise, error);
    } else if (settled === FULFILLED) {
      fulfill(promise, value);
    } else if (settled === REJECTED) {
      _reject(promise, value);
    }
}

function initializePromise(promise, resolver) {
  try {
    resolver(function resolvePromise(value) {
      _resolve(promise, value);
    }, function rejectPromise(reason) {
      _reject(promise, reason);
    });
  } catch (e) {
    _reject(promise, e);
  }
}

var id = 0;
function nextId() {
  return id++;
}

function makePromise(promise) {
  promise[PROMISE_ID] = id++;
  promise._state = undefined;
  promise._result = undefined;
  promise._subscribers = [];
}

function Enumerator(Constructor, input) {
  this._instanceConstructor = Constructor;
  this.promise = new Constructor(noop);

  if (!this.promise[PROMISE_ID]) {
    makePromise(this.promise);
  }

  if (isArray(input)) {
    this._input = input;
    this.length = input.length;
    this._remaining = input.length;

    this._result = new Array(this.length);

    if (this.length === 0) {
      fulfill(this.promise, this._result);
    } else {
      this.length = this.length || 0;
      this._enumerate();
      if (this._remaining === 0) {
        fulfill(this.promise, this._result);
      }
    }
  } else {
    _reject(this.promise, validationError());
  }
}

function validationError() {
  return new Error('Array Methods must be provided an Array');
};

Enumerator.prototype._enumerate = function () {
  var length = this.length;
  var _input = this._input;

  for (var i = 0; this._state === PENDING && i < length; i++) {
    this._eachEntry(_input[i], i);
  }
};

Enumerator.prototype._eachEntry = function (entry, i) {
  var c = this._instanceConstructor;
  var resolve$$ = c.resolve;

  if (resolve$$ === resolve) {
    var _then = getThen(entry);

    if (_then === then && entry._state !== PENDING) {
      this._settledAt(entry._state, i, entry._result);
    } else if (typeof _then !== 'function') {
      this._remaining--;
      this._result[i] = entry;
    } else if (c === Promise) {
      var promise = new c(noop);
      handleMaybeThenable(promise, entry, _then);
      this._willSettleAt(promise, i);
    } else {
      this._willSettleAt(new c(function (resolve$$) {
        return resolve$$(entry);
      }), i);
    }
  } else {
    this._willSettleAt(resolve$$(entry), i);
  }
};

Enumerator.prototype._settledAt = function (state, i, value) {
  var promise = this.promise;

  if (promise._state === PENDING) {
    this._remaining--;

    if (state === REJECTED) {
      _reject(promise, value);
    } else {
      this._result[i] = value;
    }
  }

  if (this._remaining === 0) {
    fulfill(promise, this._result);
  }
};

Enumerator.prototype._willSettleAt = function (promise, i) {
  var enumerator = this;

  subscribe(promise, undefined, function (value) {
    return enumerator._settledAt(FULFILLED, i, value);
  }, function (reason) {
    return enumerator._settledAt(REJECTED, i, reason);
  });
};

/**
  `Promise.all` accepts an array of promises, and returns a new promise which
  is fulfilled with an array of fulfillment values for the passed promises, or
  rejected with the reason of the first passed promise to be rejected. It casts all
  elements of the passed iterable to promises as it runs this algorithm.

  Example:

  ```javascript
  let promise1 = resolve(1);
  let promise2 = resolve(2);
  let promise3 = resolve(3);
  let promises = [ promise1, promise2, promise3 ];

  Promise.all(promises).then(function(array){
    // The array here would be [ 1, 2, 3 ];
  });
  ```

  If any of the `promises` given to `all` are rejected, the first promise
  that is rejected will be given as an argument to the returned promises's
  rejection handler. For example:

  Example:

  ```javascript
  let promise1 = resolve(1);
  let promise2 = reject(new Error("2"));
  let promise3 = reject(new Error("3"));
  let promises = [ promise1, promise2, promise3 ];

  Promise.all(promises).then(function(array){
    // Code here never runs because there are rejected promises!
  }, function(error) {
    // error.message === "2"
  });
  ```

  @method all
  @static
  @param {Array} entries array of promises
  @param {String} label optional string for labeling the promise.
  Useful for tooling.
  @return {Promise} promise that is fulfilled when all `promises` have been
  fulfilled, or rejected if any of them become rejected.
  @static
*/
function all(entries) {
  return new Enumerator(this, entries).promise;
}

/**
  `Promise.race` returns a new promise which is settled in the same way as the
  first passed promise to settle.

  Example:

  ```javascript
  let promise1 = new Promise(function(resolve, reject){
    setTimeout(function(){
      resolve('promise 1');
    }, 200);
  });

  let promise2 = new Promise(function(resolve, reject){
    setTimeout(function(){
      resolve('promise 2');
    }, 100);
  });

  Promise.race([promise1, promise2]).then(function(result){
    // result === 'promise 2' because it was resolved before promise1
    // was resolved.
  });
  ```

  `Promise.race` is deterministic in that only the state of the first
  settled promise matters. For example, even if other promises given to the
  `promises` array argument are resolved, but the first settled promise has
  become rejected before the other promises became fulfilled, the returned
  promise will become rejected:

  ```javascript
  let promise1 = new Promise(function(resolve, reject){
    setTimeout(function(){
      resolve('promise 1');
    }, 200);
  });

  let promise2 = new Promise(function(resolve, reject){
    setTimeout(function(){
      reject(new Error('promise 2'));
    }, 100);
  });

  Promise.race([promise1, promise2]).then(function(result){
    // Code here never runs
  }, function(reason){
    // reason.message === 'promise 2' because promise 2 became rejected before
    // promise 1 became fulfilled
  });
  ```

  An example real-world use case is implementing timeouts:

  ```javascript
  Promise.race([ajax('foo.json'), timeout(5000)])
  ```

  @method race
  @static
  @param {Array} promises array of promises to observe
  Useful for tooling.
  @return {Promise} a promise which settles in the same way as the first passed
  promise to settle.
*/
function race(entries) {
  /*jshint validthis:true */
  var Constructor = this;

  if (!isArray(entries)) {
    return new Constructor(function (_, reject) {
      return reject(new TypeError('You must pass an array to race.'));
    });
  } else {
    return new Constructor(function (resolve, reject) {
      var length = entries.length;
      for (var i = 0; i < length; i++) {
        Constructor.resolve(entries[i]).then(resolve, reject);
      }
    });
  }
}

/**
  `Promise.reject` returns a promise rejected with the passed `reason`.
  It is shorthand for the following:

  ```javascript
  let promise = new Promise(function(resolve, reject){
    reject(new Error('WHOOPS'));
  });

  promise.then(function(value){
    // Code here doesn't run because the promise is rejected!
  }, function(reason){
    // reason.message === 'WHOOPS'
  });
  ```

  Instead of writing the above, your code now simply becomes the following:

  ```javascript
  let promise = Promise.reject(new Error('WHOOPS'));

  promise.then(function(value){
    // Code here doesn't run because the promise is rejected!
  }, function(reason){
    // reason.message === 'WHOOPS'
  });
  ```

  @method reject
  @static
  @param {Any} reason value that the returned promise will be rejected with.
  Useful for tooling.
  @return {Promise} a promise rejected with the given `reason`.
*/
function reject(reason) {
  /*jshint validthis:true */
  var Constructor = this;
  var promise = new Constructor(noop);
  _reject(promise, reason);
  return promise;
}

function needsResolver() {
  throw new TypeError('You must pass a resolver function as the first argument to the promise constructor');
}

function needsNew() {
  throw new TypeError("Failed to construct 'Promise': Please use the 'new' operator, this object constructor cannot be called as a function.");
}

/**
  Promise objects represent the eventual result of an asynchronous operation. The
  primary way of interacting with a promise is through its `then` method, which
  registers callbacks to receive either a promise's eventual value or the reason
  why the promise cannot be fulfilled.

  Terminology
  -----------

  - `promise` is an object or function with a `then` method whose behavior conforms to this specification.
  - `thenable` is an object or function that defines a `then` method.
  - `value` is any legal JavaScript value (including undefined, a thenable, or a promise).
  - `exception` is a value that is thrown using the throw statement.
  - `reason` is a value that indicates why a promise was rejected.
  - `settled` the final resting state of a promise, fulfilled or rejected.

  A promise can be in one of three states: pending, fulfilled, or rejected.

  Promises that are fulfilled have a fulfillment value and are in the fulfilled
  state.  Promises that are rejected have a rejection reason and are in the
  rejected state.  A fulfillment value is never a thenable.

  Promises can also be said to *resolve* a value.  If this value is also a
  promise, then the original promise's settled state will match the value's
  settled state.  So a promise that *resolves* a promise that rejects will
  itself reject, and a promise that *resolves* a promise that fulfills will
  itself fulfill.


  Basic Usage:
  ------------

  ```js
  let promise = new Promise(function(resolve, reject) {
    // on success
    resolve(value);

    // on failure
    reject(reason);
  });

  promise.then(function(value) {
    // on fulfillment
  }, function(reason) {
    // on rejection
  });
  ```

  Advanced Usage:
  ---------------

  Promises shine when abstracting away asynchronous interactions such as
  `XMLHttpRequest`s.

  ```js
  function getJSON(url) {
    return new Promise(function(resolve, reject){
      let xhr = new XMLHttpRequest();

      xhr.open('GET', url);
      xhr.onreadystatechange = handler;
      xhr.responseType = 'json';
      xhr.setRequestHeader('Accept', 'application/json');
      xhr.send();

      function handler() {
        if (this.readyState === this.DONE) {
          if (this.status === 200) {
            resolve(this.response);
          } else {
            reject(new Error('getJSON: `' + url + '` failed with status: [' + this.status + ']'));
          }
        }
      };
    });
  }

  getJSON('/posts.json').then(function(json) {
    // on fulfillment
  }, function(reason) {
    // on rejection
  });
  ```

  Unlike callbacks, promises are great composable primitives.

  ```js
  Promise.all([
    getJSON('/posts'),
    getJSON('/comments')
  ]).then(function(values){
    values[0] // => postsJSON
    values[1] // => commentsJSON

    return values;
  });
  ```

  @class Promise
  @param {function} resolver
  Useful for tooling.
  @constructor
*/
function Promise(resolver) {
  this[PROMISE_ID] = nextId();
  this._result = this._state = undefined;
  this._subscribers = [];

  if (noop !== resolver) {
    typeof resolver !== 'function' && needsResolver();
    this instanceof Promise ? initializePromise(this, resolver) : needsNew();
  }
}

Promise.all = all;
Promise.race = race;
Promise.resolve = resolve;
Promise.reject = reject;
Promise._setScheduler = setScheduler;
Promise._setAsap = setAsap;
Promise._asap = asap;

Promise.prototype = {
  constructor: Promise,

  /**
    The primary way of interacting with a promise is through its `then` method,
    which registers callbacks to receive either a promise's eventual value or the
    reason why the promise cannot be fulfilled.
  
    ```js
    findUser().then(function(user){
      // user is available
    }, function(reason){
      // user is unavailable, and you are given the reason why
    });
    ```
  
    Chaining
    --------
  
    The return value of `then` is itself a promise.  This second, 'downstream'
    promise is resolved with the return value of the first promise's fulfillment
    or rejection handler, or rejected if the handler throws an exception.
  
    ```js
    findUser().then(function (user) {
      return user.name;
    }, function (reason) {
      return 'default name';
    }).then(function (userName) {
      // If `findUser` fulfilled, `userName` will be the user's name, otherwise it
      // will be `'default name'`
    });
  
    findUser().then(function (user) {
      throw new Error('Found user, but still unhappy');
    }, function (reason) {
      throw new Error('`findUser` rejected and we're unhappy');
    }).then(function (value) {
      // never reached
    }, function (reason) {
      // if `findUser` fulfilled, `reason` will be 'Found user, but still unhappy'.
      // If `findUser` rejected, `reason` will be '`findUser` rejected and we're unhappy'.
    });
    ```
    If the downstream promise does not specify a rejection handler, rejection reasons will be propagated further downstream.
  
    ```js
    findUser().then(function (user) {
      throw new PedagogicalException('Upstream error');
    }).then(function (value) {
      // never reached
    }).then(function (value) {
      // never reached
    }, function (reason) {
      // The `PedgagocialException` is propagated all the way down to here
    });
    ```
  
    Assimilation
    ------------
  
    Sometimes the value you want to propagate to a downstream promise can only be
    retrieved asynchronously. This can be achieved by returning a promise in the
    fulfillment or rejection handler. The downstream promise will then be pending
    until the returned promise is settled. This is called *assimilation*.
  
    ```js
    findUser().then(function (user) {
      return findCommentsByAuthor(user);
    }).then(function (comments) {
      // The user's comments are now available
    });
    ```
  
    If the assimliated promise rejects, then the downstream promise will also reject.
  
    ```js
    findUser().then(function (user) {
      return findCommentsByAuthor(user);
    }).then(function (comments) {
      // If `findCommentsByAuthor` fulfills, we'll have the value here
    }, function (reason) {
      // If `findCommentsByAuthor` rejects, we'll have the reason here
    });
    ```
  
    Simple Example
    --------------
  
    Synchronous Example
  
    ```javascript
    let result;
  
    try {
      result = findResult();
      // success
    } catch(reason) {
      // failure
    }
    ```
  
    Errback Example
  
    ```js
    findResult(function(result, err){
      if (err) {
        // failure
      } else {
        // success
      }
    });
    ```
  
    Promise Example;
  
    ```javascript
    findResult().then(function(result){
      // success
    }, function(reason){
      // failure
    });
    ```
  
    Advanced Example
    --------------
  
    Synchronous Example
  
    ```javascript
    let author, books;
  
    try {
      author = findAuthor();
      books  = findBooksByAuthor(author);
      // success
    } catch(reason) {
      // failure
    }
    ```
  
    Errback Example
  
    ```js
  
    function foundBooks(books) {
  
    }
  
    function failure(reason) {
  
    }
  
    findAuthor(function(author, err){
      if (err) {
        failure(err);
        // failure
      } else {
        try {
          findBoooksByAuthor(author, function(books, err) {
            if (err) {
              failure(err);
            } else {
              try {
                foundBooks(books);
              } catch(reason) {
                failure(reason);
              }
            }
          });
        } catch(error) {
          failure(err);
        }
        // success
      }
    });
    ```
  
    Promise Example;
  
    ```javascript
    findAuthor().
      then(findBooksByAuthor).
      then(function(books){
        // found books
    }).catch(function(reason){
      // something went wrong
    });
    ```
  
    @method then
    @param {Function} onFulfilled
    @param {Function} onRejected
    Useful for tooling.
    @return {Promise}
  */
  then: then,

  /**
    `catch` is simply sugar for `then(undefined, onRejection)` which makes it the same
    as the catch block of a try/catch statement.
  
    ```js
    function findAuthor(){
      throw new Error('couldn't find that author');
    }
  
    // synchronous
    try {
      findAuthor();
    } catch(reason) {
      // something went wrong
    }
  
    // async with promises
    findAuthor().catch(function(reason){
      // something went wrong
    });
    ```
  
    @method catch
    @param {Function} onRejection
    Useful for tooling.
    @return {Promise}
  */
  'catch': function _catch(onRejection) {
    return this.then(null, onRejection);
  }
};

function polyfill() {
    var local = undefined;

    if (typeof global !== 'undefined') {
        local = global;
    } else if (typeof self !== 'undefined') {
        local = self;
    } else {
        try {
            local = Function('return this')();
        } catch (e) {
            throw new Error('polyfill failed because global object is unavailable in this environment');
        }
    }

    var P = local.Promise;

    if (P) {
        var promiseToString = null;
        try {
            promiseToString = Object.prototype.toString.call(P.resolve());
        } catch (e) {
            // silently ignored
        }

        if (promiseToString === '[object Promise]' && !P.cast) {
            return;
        }
    }

    local.Promise = Promise;
}

// Strange compat..
Promise.polyfill = polyfill;
Promise.Promise = Promise;

return Promise;

})));
//# sourceMappingURL=es6-promise.map

/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(17), __webpack_require__(18)))

/***/ }),
/* 7 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __assign = (this && this.__assign) || Object.assign || function(t) {
    for (var s, i = 1, n = arguments.length; i < n; i++) {
        s = arguments[i];
        for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
            t[p] = s[p];
    }
    return t;
};
Object.defineProperty(exports, "__esModule", { value: true });
var json_1 = __webpack_require__(0);
var deathTimeField = '__deathTime';
var BrowserStorage = (function () {
    function BrowserStorage(storage) {
        this.storage = storage;
    }
    BrowserStorage.prototype.set = function (key, value, timeToLive) {
        if (typeof value !== 'object') {
            return;
        }
        var storeItem = __assign({}, value);
        if (timeToLive) {
            var deathTime = Date.now() + timeToLive;
            storeItem[deathTimeField] = deathTime;
        }
        this.storage.setItem(key, json_1.default.stringify(storeItem));
    };
    BrowserStorage.prototype.get = function (key) {
        var itemStr = this.storage.getItem(key);
        if (!itemStr) {
            return null;
        }
        var storeItem = json_1.default.parse(itemStr);
        if (typeof storeItem !== 'object') {
            return null;
        }
        if (storeItem[deathTimeField] < Date.now()) {
            this.storage.removeItem(key);
            return null;
        }
        return storeItem;
    };
    BrowserStorage.prototype.remove = function (key) {
        this.storage.removeItem(key);
    };
    return BrowserStorage;
}());
;
exports.default = BrowserStorage;
var SessionStorage = (function (_super) {
    __extends(SessionStorage, _super);
    function SessionStorage() {
        return _super.call(this, window.sessionStorage) || this;
    }
    return SessionStorage;
}(BrowserStorage));
exports.SessionStorage = SessionStorage;
exports.sessionStorage = new SessionStorage();
var LocalStorage = (function (_super) {
    __extends(LocalStorage, _super);
    function LocalStorage() {
        return _super.call(this, window.localStorage) || this;
    }
    return LocalStorage;
}(BrowserStorage));
exports.LocalStorage = LocalStorage;
exports.localStorage = new LocalStorage();


/***/ }),
/* 8 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var browserStorage_1 = __webpack_require__(7);
var cookieStorage_1 = __webpack_require__(20);
describe('Blades Temp storage tests', function () {
    describe('Local storage test', function () {
        testScript(browserStorage_1.localStorage);
    });
    describe('Session storage test', function () {
        testScript(browserStorage_1.sessionStorage);
    });
    describe('Cookie storage test', function () {
        testScript(cookieStorage_1.default);
    });
});
function testScript(storage) {
    it('Add item', function () {
        var item = { a: 5 };
        storage.set('item', item);
        var savedItem = storage.get('item');
        expect(savedItem['a']).toBe(5);
    });
    it('Add item with expired', function (done) {
        var item = { a: 5 };
        storage.set('item', item, 150);
        setTimeout(function () {
            var savedItem = storage.get('item');
            expect(savedItem['a']).toBe(5);
        }, 50);
        setTimeout(function () {
            var savedItem = storage.get('item');
            expect(savedItem).toBeNull();
            done();
        }, 250);
    });
    it('Remove item', function () {
        var item = { a: 6 };
        storage.set('item', item);
        var savedItem = storage.get('item');
        expect(savedItem['a']).toBe(6);
        storage.remove('item');
        var savedItem2 = storage.get('item');
        expect(savedItem2).toBeNull();
    });
}
;


/***/ }),
/* 9 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

//code based on https://learn.javascript.ru/cookie
Object.defineProperty(exports, "__esModule", { value: true });
var CookieOptions = (function () {
    function CookieOptions(expires) {
        this.expires = expires;
        this.path = '/';
    }
    return CookieOptions;
}());
exports.CookieOptions = CookieOptions;
var CookieApi = (function () {
    function CookieApi() {
    }
    CookieApi.prototype.getCookie = function (name) {
        var matches = document.cookie.match(new RegExp("(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"));
        return matches ? decodeURIComponent(matches[1]) : undefined;
    };
    CookieApi.prototype.setCookie = function (name, value, options) {
        options = options || new CookieOptions();
        var expires = options.expires;
        if (typeof expires == "number" && expires) {
            var d = new Date();
            d.setTime(d.getTime() + expires);
            expires = options.expires = d;
        }
        if (expires && expires.toUTCString) {
            options.expires = expires.toUTCString();
        }
        value = encodeURIComponent(value);
        var updatedCookie = name + "=" + value;
        for (var propName in options) {
            updatedCookie += "; " + propName;
            var propValue = options[propName];
            if (propValue !== true) {
                updatedCookie += "=" + propValue;
            }
        }
        document.cookie = updatedCookie;
    };
    CookieApi.prototype.deleteCookie = function (name) {
        this.setCookie(name, '', new CookieOptions(-1));
    };
    return CookieApi;
}());
exports.CookieApi = CookieApi;
var cookie = new CookieApi();
exports.default = cookie;


/***/ }),
/* 10 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var RequestExecutionError = (function () {
    function RequestExecutionError(source) {
        this.status = typeof (source.status) === 'number' ? source.status : 499;
        this.statusText = typeof (source.statusText) === 'string' ? source.statusText : 'UNKNOWN';
        this.message = typeof (source.responseText) === 'string' ? source.responseText :
            (typeof (source.message) === 'string' ? source.message : 'Неизвестная ошибка');
        this.message = this.processJson(this.message);
    }
    RequestExecutionError.prototype.processJson = function (message) {
        var result = message;
        try {
            var mesObject = JSON.parse(message);
            if (typeof (mesObject.error) === 'string') {
                result = mesObject.error;
            }
            if (typeof (mesObject.error_description) === 'string') {
                result = mesObject.error_description;
            }
        }
        catch (e) {
            //ignore
        }
        return result;
    };
    return RequestExecutionError;
}());
exports.default = RequestExecutionError;
;


/***/ }),
/* 11 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var tokenInfo_1 = __webpack_require__(13);
var xhr_1 = __webpack_require__(3);
var noop_1 = __webpack_require__(1);
var cookie_1 = __webpack_require__(9);
var Auth = (function () {
    function Auth(accessTokenInfoStorageKey, accessTokenCookieName) {
        this.accessTokenPath = '/token';
        this.tokenInfoChangedHandlers = [];
        this.accessTokenInfoStorageKey = accessTokenInfoStorageKey;
        this.accessTokenCookieName = accessTokenCookieName;
    }
    Auth.prototype.requestNewAccessToken = function (login, password) {
        var data = "grant_type=password&username=" + login + "&password=" + password;
        var date = new Date();
        var xhr = new xhr_1.default(this.accessTokenPath, 'POST', data);
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=utf-8');
        return xhr.execute().then(function (result) {
            var info = new tokenInfo_1.default();
            info.token = result.access_token;
            info.login = login;
            date.setSeconds(date.getSeconds() + result.expires_in);
            info.expireDate = date;
            return info;
        });
    };
    Auth.prototype.getTokenInfo = function () {
        var tokenInfoJson = localStorage.getItem(this.accessTokenInfoStorageKey);
        if (!tokenInfoJson) {
            return null;
        }
        var tokenInfo = tokenInfo_1.default.fromJson(tokenInfoJson);
        return tokenInfo;
    };
    Auth.prototype.addAccessTokenToRequestHeader = function (xhr) {
        var tokenInfo = this.getTokenInfo();
        if (!tokenInfo) {
            return xhr;
        }
        xhr.setRequestHeader('Authorization', "Bearer " + tokenInfo.token);
        return xhr;
    };
    Auth.prototype.authorize = function (login, password) {
        var _this = this;
        return this.requestNewAccessToken(login, password).then(function (tokenInfo) {
            localStorage.setItem(_this.accessTokenInfoStorageKey, tokenInfo.toJson());
            cookie_1.default.setCookie(_this.accessTokenCookieName, tokenInfo.token, new cookie_1.CookieOptions(tokenInfo.expireDate));
            _this.tokenInfoChanged(tokenInfo);
            return tokenInfo;
        });
    };
    Auth.prototype.clearTokenInfo = function () {
        localStorage.removeItem(this.accessTokenInfoStorageKey);
        cookie_1.default.deleteCookie(this.accessTokenCookieName);
        this.tokenInfoChanged(null);
    };
    Auth.prototype.onTokenInfoChanged = function (handler) {
        var _this = this;
        if (!handler) {
            return noop_1.default;
        }
        this.tokenInfoChangedHandlers.push(handler);
        return function () {
            var index = _this.tokenInfoChangedHandlers.indexOf(handler);
            if (index > -1) {
                _this.tokenInfoChangedHandlers.splice(index, 1);
            }
        };
    };
    Auth.prototype.tokenInfoChanged = function (newInfo) {
        this.tokenInfoChangedHandlers.forEach(function (handler) { return handler(newInfo); });
    };
    return Auth;
}());
exports.Auth = Auth;
;
var auth = new Auth('accessTokenInfo', 'accessToken');
exports.default = auth;


/***/ }),
/* 12 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var xhr_1 = __webpack_require__(3);
var serverConnection_1 = __webpack_require__(2);
var json_1 = __webpack_require__(0);
var BaseOperation = (function () {
    function BaseOperation(name, data, requestType, requestMethod) {
        this.requestUrl = '/api/operation';
        this.name = name;
        this.data = data;
        this.requestMethod = requestMethod;
        this.requestType = requestType;
    }
    BaseOperation.prototype.createXhr = function () {
        var xhr = new xhr_1.default(this.requestUrl, this.requestMethod, this.data);
        xhr.setRequestHeader('x-blades-operation-name', this.name);
        xhr.setRequestHeader('x-blades-operation-request-type', this.requestType);
        return xhr;
    };
    return BaseOperation;
}());
exports.default = BaseOperation;
var JsonOperation = (function (_super) {
    __extends(JsonOperation, _super);
    function JsonOperation(name, data) {
        var _this = this;
        var strData = json_1.default.stringify(data);
        _this = _super.call(this, name, strData, 'JsonOperation', 'POST') || this;
        return _this;
    }
    JsonOperation.prototype.execute = function () {
        var xhr = this.createXhr();
        xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
        return xhr.execute();
    };
    JsonOperation.prototype.executeViaConnection = function () {
        var message = {
            name: this.name,
            requestType: this.requestType,
            data: this.data
        };
        var messageStr = this.requestType + "@@@@@" + this.name + "@@@@@" + this.data;
        serverConnection_1.default.send(messageStr);
    };
    return JsonOperation;
}(BaseOperation));
exports.JsonOperation = JsonOperation;
var FormDataOperation = (function (_super) {
    __extends(FormDataOperation, _super);
    function FormDataOperation(name, data) {
        return _super.call(this, name, data, 'FormDataOperation', 'POST') || this;
    }
    return FormDataOperation;
}(BaseOperation));
exports.FormDataOperation = FormDataOperation;


/***/ }),
/* 13 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var json_1 = __webpack_require__(0);
var TokenInfo = (function () {
    function TokenInfo() {
    }
    TokenInfo.prototype.toJson = function () {
        return json_1.default.stringify(this);
    };
    TokenInfo.fromJson = function (jsonStr) {
        var obj = json_1.default.parse(jsonStr);
        if (!obj) {
            return null;
        }
        var result = new TokenInfo();
        result.login = obj.login;
        result.token = obj.token;
        result.expireDate = new Date(obj.expireDate);
        return result;
    };
    return TokenInfo;
}());
exports.default = TokenInfo;


/***/ }),
/* 14 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var serverOperations_1 = __webpack_require__(12);
var serverConnection_1 = __webpack_require__(2);
var auth_1 = __webpack_require__(11);
var Tuple = (function () {
    function Tuple() {
    }
    return Tuple;
}());
var ComplexDataType = (function () {
    function ComplexDataType() {
    }
    return ComplexDataType;
}());
describe('Xhr tests', function () {
    beforeEach(function () {
        auth_1.default.clearTokenInfo();
    });
    it('TestOperation.EchoOperation', function (done) {
        var operation = new serverOperations_1.JsonOperation('TestOperation.EchoOperation', 5);
        operation.execute().then(function (result) {
            expect(result).toBe(5);
            done();
        }).catch(function (err) {
            console.error(err);
            expect(false).toBeTruthy();
            done();
        });
    });
    it('TestOperation.ComplexDataOperation', function (done) {
        var data = new ComplexDataType();
        data.someInt = -3;
        data.someString = 'Тестовая строка';
        data.someBool = false;
        data.time = new Date();
        data.listData = [{ item1: 5, item2: 'Abc', item3: true }, { item1: -5, item2: 'abC', item3: false }];
        new serverOperations_1.JsonOperation('TestOperation.ComplexDataOperation', data).execute()
            .then(function (result) {
            expect(typeof (result.someInt)).toBe('number');
            expect(result.someInt).toBe(-6);
            expect(result.someString).toBe('ТЕСТОВАЯ СТРОКА');
            expect(typeof (result.someBool)).toBe('boolean');
            expect(result.someBool).toBe(true);
            expect(typeof (result.time)).toBe('object');
            expect(result.time.getHours()).toBe(data.time.getHours());
            expect(result.time.getFullYear()).toBe(data.time.getFullYear() + 1);
            expect(result.listData.length).toBe(2);
            expect(result.listData[0].item1).toBe(4);
            expect(result.listData[1].item2).toBe('abc');
            done();
        })
            .catch(function (err) {
            console.error(err);
            expect(false).toBeTruthy();
            done();
        });
    });
    it('TestOperation.FailedOperation 0', function (done) {
        var operation = new serverOperations_1.JsonOperation('TestOperation.FailedOperation', 0);
        operation.execute().then(function (result) {
            expect(result).toBe(0);
            done();
        }).catch(function (err) {
            console.error(err);
            expect(false).toBeTruthy();
            done();
        });
    });
    it('TestOperation.FailedOperation > 0', function (done) {
        var operation = new serverOperations_1.JsonOperation('TestOperation.FailedOperation', 5);
        operation.execute().then(function (result) {
            expect(result).toBe(0);
            done();
        }).catch(function (err) {
            expect(err.status).toBe(500);
            expect(err.message).toBe('Data > 0');
            done();
        });
    });
    it('TestOperation.FailedOperation < 0', function (done) {
        var operation = new serverOperations_1.JsonOperation('TestOperation.FailedOperation', -5);
        operation.execute().then(function (result) {
            expect(result).toBe(0);
            done();
        }).catch(function (err) {
            expect(err.status).toBe(500);
            expect(err.message).toBe('Data < 0');
            done();
        });
    });
    it('Login password good info test', function (done) {
        auth_1.default.authorize('admin', 'w')
            .then(function (data) {
            expect(data.login).toBe('admin');
            done();
        })
            .catch(function (err) {
            expect(false).toBeTruthy();
            done();
        });
    });
    it('Login password bad info test', function (done) {
        auth_1.default.authorize('admin', 'p')
            .then(function (data) {
            expect('').toBe('Попытка должна провалиться');
            done();
        })
            .catch(function (err) {
            expect(err.status).toBe(400);
            expect(err.message).toBe("Неверный логин или пароль");
            done();
        });
    });
    it('TestOperation.AuthFailedOperation Bad scenario', function (done) {
        var operation = new serverOperations_1.JsonOperation('TestOperation.AuthFailedOperation', 5);
        operation.execute().then(function (result) {
            expect(result).toBe('Попытка должна провалиться');
            done();
        }).catch(function (err) {
            expect(err.status).toBe(401);
            expect(err.message).toBe('Пользователь не авторизован');
            done();
        });
    });
    it('TestOperation.AuthFailedOperation Good scenario', function (done) {
        var operation = new serverOperations_1.JsonOperation('TestOperation.AuthFailedOperation', 7);
        auth_1.default.authorize('admin', 'w').then(function () {
            operation.execute().then(function (result) {
                expect(result).toBe(7);
                done();
            }).catch(function (err) {
                expect(false).toBeTruthy();
                done();
            });
        });
    });
    it('TestOperation.PermissionedFailedOperation Bad scenario', function (done) {
        var operation = new serverOperations_1.JsonOperation('TestOperation.PermissionedFailedOperation', 7);
        auth_1.default.authorize('someUser', 'w').then(function () {
            operation.execute().then(function (result) {
                expect(result).toBe('Попытка должна провалиться');
                done();
            }).catch(function (err) {
                expect(err.status).toBe(403);
                expect(err.message).toBe("Пользователь someuser не имеет прав: Update, Delete, на ресурс: Тестовый тип --- Тестовый объект");
                done();
            });
        });
    });
    it('TestOperation.PermissionedFailedOperation Good scenario', function (done) {
        var operation = new serverOperations_1.JsonOperation('TestOperation.PermissionedFailedOperation', 7);
        auth_1.default.authorize('admin', 'w').then(function () {
            operation.execute().then(function (result) {
                expect(result).toBe(7);
                done();
            }).catch(function (err) {
                expect(false).toBeTruthy();
                done();
            });
        });
    });
});
describe('Web socket connection Tests', function () {
    it('TestOperation.WebSocketOperation; test 1', function (done) {
        var data = new Tuple();
        data.item1 = 4;
        data.item2 = 'TesT';
        data.item3 = new Date();
        var operation = new serverOperations_1.JsonOperation('TestOperation.WebSocketOperation', data);
        auth_1.default.authorize('admin', 'w').then(function () {
            serverConnection_1.default.open();
            operation.executeViaConnection();
        });
        serverConnection_1.default.onNotifyMessage('TestWsOp', function (newData) {
            expect(newData.item1).toBe(8);
            expect(newData.item2).toBe('test');
            expect(newData.item3.getFullYear()).toBe(data.item3.getFullYear() + 5);
            done();
        });
    });
});


/***/ }),
/* 15 */,
/* 16 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
__webpack_require__(5);
__webpack_require__(4);


/***/ }),
/* 17 */
/***/ (function(module, exports) {

// shim for using process in browser
var process = module.exports = {};

// cached from whatever global is present so that test runners that stub it
// don't break things.  But we need to wrap it in a try catch in case it is
// wrapped in strict mode code which doesn't define any globals.  It's inside a
// function because try/catches deoptimize in certain engines.

var cachedSetTimeout;
var cachedClearTimeout;

function defaultSetTimout() {
    throw new Error('setTimeout has not been defined');
}
function defaultClearTimeout () {
    throw new Error('clearTimeout has not been defined');
}
(function () {
    try {
        if (typeof setTimeout === 'function') {
            cachedSetTimeout = setTimeout;
        } else {
            cachedSetTimeout = defaultSetTimout;
        }
    } catch (e) {
        cachedSetTimeout = defaultSetTimout;
    }
    try {
        if (typeof clearTimeout === 'function') {
            cachedClearTimeout = clearTimeout;
        } else {
            cachedClearTimeout = defaultClearTimeout;
        }
    } catch (e) {
        cachedClearTimeout = defaultClearTimeout;
    }
} ())
function runTimeout(fun) {
    if (cachedSetTimeout === setTimeout) {
        //normal enviroments in sane situations
        return setTimeout(fun, 0);
    }
    // if setTimeout wasn't available but was latter defined
    if ((cachedSetTimeout === defaultSetTimout || !cachedSetTimeout) && setTimeout) {
        cachedSetTimeout = setTimeout;
        return setTimeout(fun, 0);
    }
    try {
        // when when somebody has screwed with setTimeout but no I.E. maddness
        return cachedSetTimeout(fun, 0);
    } catch(e){
        try {
            // When we are in I.E. but the script has been evaled so I.E. doesn't trust the global object when called normally
            return cachedSetTimeout.call(null, fun, 0);
        } catch(e){
            // same as above but when it's a version of I.E. that must have the global object for 'this', hopfully our context correct otherwise it will throw a global error
            return cachedSetTimeout.call(this, fun, 0);
        }
    }


}
function runClearTimeout(marker) {
    if (cachedClearTimeout === clearTimeout) {
        //normal enviroments in sane situations
        return clearTimeout(marker);
    }
    // if clearTimeout wasn't available but was latter defined
    if ((cachedClearTimeout === defaultClearTimeout || !cachedClearTimeout) && clearTimeout) {
        cachedClearTimeout = clearTimeout;
        return clearTimeout(marker);
    }
    try {
        // when when somebody has screwed with setTimeout but no I.E. maddness
        return cachedClearTimeout(marker);
    } catch (e){
        try {
            // When we are in I.E. but the script has been evaled so I.E. doesn't  trust the global object when called normally
            return cachedClearTimeout.call(null, marker);
        } catch (e){
            // same as above but when it's a version of I.E. that must have the global object for 'this', hopfully our context correct otherwise it will throw a global error.
            // Some versions of I.E. have different rules for clearTimeout vs setTimeout
            return cachedClearTimeout.call(this, marker);
        }
    }



}
var queue = [];
var draining = false;
var currentQueue;
var queueIndex = -1;

function cleanUpNextTick() {
    if (!draining || !currentQueue) {
        return;
    }
    draining = false;
    if (currentQueue.length) {
        queue = currentQueue.concat(queue);
    } else {
        queueIndex = -1;
    }
    if (queue.length) {
        drainQueue();
    }
}

function drainQueue() {
    if (draining) {
        return;
    }
    var timeout = runTimeout(cleanUpNextTick);
    draining = true;

    var len = queue.length;
    while(len) {
        currentQueue = queue;
        queue = [];
        while (++queueIndex < len) {
            if (currentQueue) {
                currentQueue[queueIndex].run();
            }
        }
        queueIndex = -1;
        len = queue.length;
    }
    currentQueue = null;
    draining = false;
    runClearTimeout(timeout);
}

process.nextTick = function (fun) {
    var args = new Array(arguments.length - 1);
    if (arguments.length > 1) {
        for (var i = 1; i < arguments.length; i++) {
            args[i - 1] = arguments[i];
        }
    }
    queue.push(new Item(fun, args));
    if (queue.length === 1 && !draining) {
        runTimeout(drainQueue);
    }
};

// v8 likes predictible objects
function Item(fun, array) {
    this.fun = fun;
    this.array = array;
}
Item.prototype.run = function () {
    this.fun.apply(null, this.array);
};
process.title = 'browser';
process.browser = true;
process.env = {};
process.argv = [];
process.version = ''; // empty string to avoid regexp issues
process.versions = {};

function noop() {}

process.on = noop;
process.addListener = noop;
process.once = noop;
process.off = noop;
process.removeListener = noop;
process.removeAllListeners = noop;
process.emit = noop;
process.prependListener = noop;
process.prependOnceListener = noop;

process.listeners = function (name) { return [] }

process.binding = function (name) {
    throw new Error('process.binding is not supported');
};

process.cwd = function () { return '/' };
process.chdir = function (dir) {
    throw new Error('process.chdir is not supported');
};
process.umask = function() { return 0; };


/***/ }),
/* 18 */
/***/ (function(module, exports) {

var g;

// This works in non-strict mode
g = (function() {
	return this;
})();

try {
	// This works if eval is allowed (see CSP)
	g = g || Function("return this")() || (1,eval)("this");
} catch(e) {
	// This works if the window reference is available
	if(typeof window === "object")
		g = window;
}

// g can still be undefined, but nothing to do about it...
// We return undefined, instead of nothing here, so it's
// easier to handle this case. if(!global) { ...}

module.exports = g;


/***/ }),
/* 19 */
/***/ (function(module, exports) {

/* (ignored) */

/***/ }),
/* 20 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var cookie_1 = __webpack_require__(9);
var json_1 = __webpack_require__(0);
var CookieStorage = (function () {
    function CookieStorage() {
    }
    CookieStorage.prototype.set = function (key, value, timeToLive) {
        if (typeof value !== 'object') {
            return;
        }
        var options = new cookie_1.CookieOptions(timeToLive || undefined);
        cookie_1.default.setCookie(key, json_1.default.stringify(value), options);
    };
    CookieStorage.prototype.get = function (key) {
        var itemStr = cookie_1.default.getCookie(key);
        if (!itemStr) {
            return null;
        }
        var storeItem = json_1.default.parse(itemStr);
        if (typeof storeItem !== 'object') {
            return null;
        }
        return storeItem;
    };
    CookieStorage.prototype.remove = function (key) {
        cookie_1.default.deleteCookie(key);
    };
    return CookieStorage;
}());
exports.CookieStorage = CookieStorage;
;
var cookieStorage = new CookieStorage();
exports.default = cookieStorage;


/***/ })
/******/ ]);
//# sourceMappingURL=unitTests.bundle.js.map