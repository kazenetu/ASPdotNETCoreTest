front.common.utils.extendController = function extendController(subFunc,superFunc){
    subFunc.prototype =  Object.create(superFunc.prototype);
    subFunc.prototype.constructor =  subFunc;
    superFunc.call(subFunc);
}