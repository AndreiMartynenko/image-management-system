package uk.ac.kingston.ci7250.ims.service;

import org.springframework.stereotype.Service;
import uk.ac.kingston.ci7250.ims.domain.WorkflowTask;

import java.math.BigDecimal;
import java.util.List;

@Service
public class BillingService {
    private final WorkflowService workflowService;

    public BillingService(WorkflowService workflowService) {
        this.workflowService = workflowService;
    }

    public BigDecimal totalCostForPatient(long patientId) {
        List<WorkflowTask> tasks = workflowService.listTasksForPatient(patientId);
        BigDecimal total = BigDecimal.ZERO;
        for (WorkflowTask t : tasks) {
            total = total.add(t.getCost());
        }
        return total;
    }
}
