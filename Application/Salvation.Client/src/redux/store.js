import rootReducer from "./reducers";
import createSagaMiddleware from 'redux-saga';
import { applyMiddleware, compose, createStore } from 'redux';
import rootSaga from "./profileSaga";

const sagaMiddleware = createSagaMiddleware();

export function configureStore(initialState) {
  console.log('Begin configuring store');

  const middleware = [sagaMiddleware];

  const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;
  const store = createStore(rootReducer, initialState, composeEnhancers(applyMiddleware(...middleware)));

  sagaMiddleware.run(rootSaga);

  console.log('Done configuring store');
  return store;
}

export default configureStore;
