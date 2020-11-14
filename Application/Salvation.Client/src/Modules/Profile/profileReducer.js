// Set of actions to handle managing the state
const profileReducer = (state, action) => {
  console.log('state:',  state);
  console.log('action:', action);

  function updateSpellValue() {
    return {
      ...state.currentProfile, spells: state.currentProfile.spells.map((spell, index) => {
        if (spell.id === action.data.spell.id) {
          return { ...spell, value: action.data.newValue };
        }
        return spell;
      })
    };
  }

  switch (action.type) {
    case 'setProfile': {
      return {
        ...state,
        currentProfile: action.data,
      };
    }
    case 'updateSpellValue': {
      return { ...state, currentProfile: updateSpellValue() };
    }
    default:
      return state;
  }
};

export default profileReducer;