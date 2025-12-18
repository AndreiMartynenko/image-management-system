package uk.ac.kingston.ci7250.ims.web.dto;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import uk.ac.kingston.ci7250.ims.domain.Modality;

public class UploadImageRequest {
    @NotNull
    private Modality modality;

    @NotBlank
    private String diseaseTag;

    public Modality getModality() {
        return modality;
    }

    public void setModality(Modality modality) {
        this.modality = modality;
    }

    public String getDiseaseTag() {
        return diseaseTag;
    }

    public void setDiseaseTag(String diseaseTag) {
        this.diseaseTag = diseaseTag;
    }
}
