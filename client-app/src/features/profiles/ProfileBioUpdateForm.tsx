import React, { useContext } from "react";
import { observer } from "mobx-react-lite";
import { Button, Form } from "semantic-ui-react";
import { Form as FinalForm, Field } from "react-final-form";
import TextInput from "../../app/common/form/TextInput";
import TextAreaInput from "../../app/common/form/TextAreaInput";
import { combineValidators, isRequired } from "revalidate";
import { RootStoreContext } from "../../app/stores/rootStore";

const validate = combineValidators({
  displayName: isRequired({ message: "Display name is required!" })
});

interface IProps {
  setUpdateBioMode: (update: boolean) => void;
}

const ProfileBioUpdateForm: React.FC<IProps> = ({ setUpdateBioMode }) => {
  const rootStore = useContext(RootStoreContext);
  const { profile, loading, updateProfile } = rootStore.profileStore;

  const handleFinalFormSubmit = (values: any) => {
    let profile = {
      displayName: values.displayName,
      bio: values.bio
    };
    updateProfile(profile).then(() => setUpdateBioMode(false));
  };

  return (
    <FinalForm
      validate={validate}
      initialValues={profile!}
      onSubmit={handleFinalFormSubmit}
      render={({ handleSubmit, invalid }) => (
        <Form onSubmit={handleSubmit} loading={loading}>
          <Field
            name="displayName"
            placeholder="Display name"
            value={profile?.displayName}
            component={TextInput}
          />
          <Field
            name="bio"
            placeholder="Bio"
            rows={3}
            value={profile!.bio}
            component={TextAreaInput}
          />
          <Button
            loading={loading}
            disabled={loading || invalid}
            fluid
            positive
            type="submit"
            content="Submit"
          />
        </Form>
      )}
    />
  );
};

export default observer(ProfileBioUpdateForm);
